# Spec: Resilient cache fallback on GET /api/tasks/upcoming

**Issue:** #54  
**Author:** @dilterporto  
**Status:** Implementing  
**Agent:** engineer  

---

## Context

Load testing with 50 req/s produced 21 HTTP 500 errors, all concentrated in `GET /api/tasks/upcoming`. Every other endpoint returned zero errors. Root cause analysis of `GetUpcomingTasksQueryHandler` identified three defects:

1. `CacheManager.ContainsKey` uses the synchronous StackExchange.Redis API (`database.StringGet`). Under connection pressure, StackExchange.Redis throws `RedisTimeoutException` or `RedisConnectionException` synchronously, bypassing the `Result<T>` chain and propagating an unhandled exception → 500.
2. No fallback path exists when any Redis operation throws — the exception escapes MediatR and FastEndpoints returns 500 with no log context.
3. `GetUpcomingTasksFromCache` accesses `Maybe<T>.Value` without checking `HasValue`, causing `InvalidOperationException` in the race window between `ContainsKey` returning `true` and `Get` returning `None`.

If left unfixed, any Redis instability (network blip, connection pool exhaustion, node failover) takes down the most-called read endpoint.

---

## Specification

### What will be built

Defensive error handling in the `GET /api/tasks/upcoming` read path so that **a Redis failure at any point — key check, read, or write — results in a silent fallback to the database projection**, never a 500.

The change is confined to `Tasks.Abstractions` (`CacheManager`) and `Tasks.Application` (`GetUpcomingTasksQueryHandler`). No new libraries. No schema changes.

### Inputs and outputs

```
Input:  GET /api/tasks/upcoming  (no body, no query params)

Output (success):       200 OK  — UpcomingTasksResponse (same shape as today)
Output (Redis failure): 200 OK  — UpcomingTasksResponse sourced from ProjectionsDbContext
Output (DB failure):    remains a failure result → FastEndpoints returns its current behaviour
```

### Behavior

- **Must** catch `RedisException` (and derived types) in all `CacheManager` operations and treat them as a cache miss, never re-throwing
- **Must** make `ContainsKey` async (`Task<bool>`) and use `KeyExistsAsync` instead of the synchronous `StringGet`
- **Must** guard `Maybe<T>.Value` in `GetUpcomingTasksFromCache` with a `HasValue` check before accessing the value; return `Maybe<T>.None` when the key is absent
- **Must** make `SetInCache` awaitable and surround the `cacheManager.Set` call in a try/catch that logs a warning on failure without surfacing the exception to the caller
- **Must** fall back to `GetUpcomingTasksFromProjections` whenever the cache path returns `None` or throws
- **Must not** change the response contract (`UpcomingTasksResponse` shape and HTTP status codes)
- **Must not** add a dependency from `Application` → `Persistence` directly
- **Should** log a structured warning (via `ILogger`) when a Redis operation fails, including the exception type, so the fallback is observable in Seq

---

## Acceptance Criteria

- [ ] AC-1: When Redis throws on `ContainsKey`, the handler returns `200 OK` with data from the database projection
- [ ] AC-2: When Redis throws on `Get`, the handler returns `200 OK` with data from the database projection
- [ ] AC-3: When Redis throws on `Set` (cache write), the handler returns `200 OK` with the response already built; a warning is logged
- [ ] AC-4: When Redis returns `None` for a key that `ContainsKey` reported as present (race), the handler does not throw and falls back to the database
- [ ] AC-5: `ICacheManager.ContainsKey` signature becomes `Task<bool>` — all call sites updated accordingly
- [ ] AC-6: `CacheValidationPipelineBehavior` (uses `ICacheManager.Invalidate`) continues to compile and function correctly after the interface change
- [ ] AC-7: The load test `npm run load:local` produces zero HTTP 500 responses on `GET /api/tasks/upcoming` at 50 req/s with Redis running

---

## Technical Constraints

- **Layers affected:** `Tasks.Abstractions` (CacheManager, ICacheManager), `Tasks.Application` (GetUpcomingTasksQueryHandler)
- **Patterns:** handler must keep the `Result<T>` / `Maybe<T>` chain from `CSharpFunctionalExtensions`; no raw `try/catch` in the handler body — wrap at `CacheManager` level
- **Interface change:** `ICacheManager.ContainsKey` → `Task<bool>`; `NullCacheManager` must be updated to match
- **Must not** introduce a circuit breaker library (Polly) in this fix — that belongs in a separate resilience spec if needed
- **Must not** change `GetUpcomingTasksEndpoint` or any layer above `Application`
- **Must not** alter the event store or projection write path

---

## Out of Scope

- General Redis circuit breaker / retry policy (Polly) — separate concern
- Pagination or date-window filtering on the upcoming endpoint
- Cache warming on startup
- Changing the cache TTL or key strategy

---

## Sensors

- [ ] Test: `GetUpcomingTasksQueryHandler_WhenRedisThrowsOnContainsKey_ReturnsDatabaseResult` passes
- [ ] Test: `GetUpcomingTasksQueryHandler_WhenRedisThrowsOnGet_ReturnsDatabaseResult` passes
- [ ] Test: `GetUpcomingTasksQueryHandler_WhenRedisThrowsOnSet_ReturnsResponseAndLogsWarning` passes
- [ ] Test: `GetUpcomingTasksQueryHandler_WhenCacheMaybeIsNone_FallsBackWithoutThrowing` passes
- [ ] Test: `CacheManager_WhenRedisThrows_ContainsKeyReturnsFalse` passes
- [ ] Test: `CacheManager_WhenRedisThrows_GetReturnsMaybeNone` passes
- [ ] Architecture: no new dependency from `Application` → `Persistence`
- [ ] Convention: `NullCacheManager` implements updated `ICacheManager` interface (compilation gate)
- [ ] Load: zero `http.codes.500` on `GET /api/tasks/upcoming` in `load-local.json` at spike phase
