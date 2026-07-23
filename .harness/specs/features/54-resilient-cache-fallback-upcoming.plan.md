# Plan: 54 — Resilient cache fallback on GET /api/tasks/upcoming

**Spec:** `.harness/specs/features/54-resilient-cache-fallback-upcoming.md`  
**Status:** Draft  
**Created:** 2026-07-22  

## Tasks

- [ ] **task-1** · agent: `engineer`  
  Make `ICacheManager.ContainsKey` async (`Task<bool>`) and update `CacheManager`, `NullCacheManager`, and `CacheValidationPipelineBehavior` to compile against the new signature.  
  _Covers: AC-5, AC-6_  
  _depends on: —_

- [ ] **task-2** · agent: `engineer`  
  Implement defensive `RedisException` handling in `CacheManager` (ContainsKey, Get, Set) with silent fallback and warning logging, and rewrite `GetUpcomingTasksQueryHandler` to fall back to `ProjectionsDbContext` when the cache returns `None` or throws.  
  _Covers: AC-1, AC-2, AC-3, AC-4_  
  _depends on: task-1_

- [ ] **task-3** · agent: `engineer`  
  Write unit tests covering Redis failure scenarios (ContainsKey, Get, Set) and the race condition (Maybe None without throw) in `GetUpcomingTasksQueryHandler` and `CacheManager`.  
  _Covers: AC-1, AC-2, AC-3, AC-4 (sensors)_  
  _depends on: task-2_

- [ ] **task-4** · agent: `engineer`  
  Run `npm run load:local` against the patched API and confirm `http.codes.500` is zero on `/api/tasks/upcoming` during the spike phase.  
  _Covers: AC-7_  
  _depends on: task-3_
