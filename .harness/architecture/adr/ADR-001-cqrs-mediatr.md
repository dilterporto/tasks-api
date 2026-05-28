# ADR-001: CQRS via MediatR

**Status:** Accepted  
**Date:** 2024-01-01

## Context

The API needs a clear separation between state-mutating operations (commands) and state-reading operations (queries). Without this separation, handlers tend to mix read and write concerns, making them harder to test, evolve independently, and reason about at scale.

## Decision

Use MediatR to implement CQRS. Commands and queries are plain C# records that implement `ICommand<T>` or `IQuery<T>` (thin wrappers over `IRequest<T>`). Handlers implement `ICommandHandler<TCommand, TResult>` or `IQueryHandler<TQuery, TResult>`. MediatR's pipeline behavior mechanism is used for cross-cutting concerns.

## Consequences

**Easier:**
- Commands and queries are independently testable without web infrastructure
- Cross-cutting concerns (logging, transactions, cache invalidation) are added as pipeline behaviors without touching handler code
- New use cases follow a consistent file and naming structure

**Harder:**
- Slightly more indirection than calling a service directly — debugging requires knowing MediatR's dispatch mechanism
- Each use case requires a minimum of two files (command + handler), even for simple cases

## Alternatives considered

- **Service classes**: Direct service injection would be simpler but mixes read/write responsibilities and complicates adding pipeline behaviors
- **Minimal API handlers as use cases**: Keeps logic close to the endpoint but makes testing harder and grows controllers/endpoints with business logic
