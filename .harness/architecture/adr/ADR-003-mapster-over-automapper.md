# ADR-003: Mapster replaces AutoMapper

**Status:** Accepted  
**Date:** 2024-06-01

## Context

AutoMapper was the original object mapping library. It was replaced for two reasons:

1. **Security vulnerability**: CVE GHSA-rvv3-g6hj-g44x affects AutoMapper < 15.1.1 and 16.0.0–16.1.0
2. **Licensing change**: AutoMapper 15.x+ became a paid commercial library (free tier removed)
3. **Dependency conflict**: AutoMapper 15.x+ requires `Microsoft.Extensions.*` 10.x, which conflicts with EF Core 8's requirement for `Microsoft.Extensions.*` 8.x, causing NU1605 downgrade errors that cannot be resolved in Central Package Management

## Decision

Replace AutoMapper with Mapster 7.4.0 (MIT license). Mappings are defined in `Tasks.Application/Mappings/TaskProfile.cs` implementing Mapster's `IRegister` interface. The project injects `MapsterMapper.IMapper` (aliased as `IMapper` in endpoints to avoid ambiguity with `FastEndpoints.IMapper`).

## Consequences

**Easier:**
- No licensing cost
- No security vulnerability
- Compatible with EF Core 8 (`Microsoft.Extensions.*` 8.x)
- Configuration is explicit via `TypeAdapterConfig` — less magic than AutoMapper profiles

**Harder:**
- Some AutoMapper-specific features (value resolvers, `IReverseMap`) require explicit re-mapping in Mapster
- Team needs familiarity with Mapster's `IRegister` pattern

## Alternatives considered

- **TinyMapper**: Lightweight but limited configuration options
- **Manual mapping methods**: No dependency but repetitive for deep object trees
- **AutoMapper with paid license**: Cost and continued `Microsoft.Extensions` conflict ruled it out
