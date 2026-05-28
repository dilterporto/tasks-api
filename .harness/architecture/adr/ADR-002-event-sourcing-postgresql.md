# ADR-002: Event Sourcing with custom PostgreSQL store

**Status:** Accepted  
**Date:** 2024-01-01

## Context

Task state changes (create, start, change, delete) need to be auditable and replayable. A traditional CRUD model overwrites state and loses history. Event sourcing preserves every change as an immutable fact.

The project already uses PostgreSQL for the read model; adding a dedicated event store on the same database reduces operational complexity.

## Decision

Implement a lightweight custom event store using EF Core and PostgreSQL. Domain events are serialized as JSON and stored in the `events` table. Aggregates are reconstructed by loading and replaying all events for a given `AggregateId` in version order.

Two separate `DbContext` instances are used:

- `EventsDbContext` — append-only event store (`events` table)
- `ProjectionsDbContext` — read model (`tasks` table), updated by event committers after each write

## Consequences

**Easier:**
- Full audit trail of all state changes without additional tooling
- Aggregate state is deterministic — any past state can be reconstructed by replaying to a version
- Event committers decouple the write model from the read model cleanly

**Harder:**
- Aggregate reconstruction requires a DB round-trip per load (all events for that aggregate)
- Schema migrations must be backward-compatible since old events use older schemas
- No built-in snapshotting — high-event aggregates will be slow to reconstruct (not a current concern)

## Alternatives considered

- **EventStoreDB**: Dedicated event store with richer features (projections, subscriptions), but adds an operational dependency and network hop
- **CRUD with audit log**: Simpler model but does not support event replay or full history reconstruction
