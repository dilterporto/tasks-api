# Sensor: Architecture Fitness

These checks verify that the layer dependency rules described in `overview.md` are not violated. Run them after any change to project references or `using` directives.

## Dependency rules

| Allowed | Forbidden |
|---------|-----------|
| `Application` → `Domain` | `Application` → `Persistence` |
| `Application` → `Abstractions` | `Domain` → any other project layer |
| `Persistence` → `Domain` | `Persistence` → `Application` |
| `Persistence` → `Abstractions` | `Api` → `Persistence` directly |
| `Api` → `Application` | `Api` → `Domain` directly |
| `Api` → `DependencyInjection` | |
| `DependencyInjection` → `Persistence` | |
| `DependencyInjection` → `Application` | |

## Manual checks

### Check for forbidden project references

```bash
# Application must not reference Persistence
grep -r "Tasks.Persistence" src/Tasks.Application/ --include="*.csproj"

# Domain must not reference any layer
grep -rE "Tasks\.(Application|Persistence|Api|DependencyInjection)" src/Tasks.Domain/ --include="*.csproj"

# Api must not reference Persistence directly
grep -r "Tasks.Persistence" src/Tasks.Api/ --include="*.csproj"
```

Expected output: empty (no matches).

### Check for forbidden namespace usage

```bash
# Application handlers must not use ProjectionsDbContext directly
grep -r "ProjectionsDbContext" src/Tasks.Application/ --include="*.cs"

# Application handlers must not use EventsDbContext directly
grep -r "EventsDbContext" src/Tasks.Application/ --include="*.cs"

# Application handlers must not use ITaskRepository from Persistence namespace
grep -r "Tasks.Persistence" src/Tasks.Application/ --include="*.cs"
```

Expected output: empty (no matches).

## Result pattern check

All command handlers must return `Result<T>`:

```bash
grep -rL "Result<" src/Tasks.Application/UseCases/ --include="*Handler.cs"
```

Expected output: empty (all handler files contain `Result<`).

## Automated enforcement (future)

These checks are candidates for a GitHub Actions step or an ArchUnitNET test suite. Until then, run them manually before opening a PR for any change that touches project references or adds new using directives across layers.
