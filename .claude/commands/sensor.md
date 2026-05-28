Run the architecture fitness checks defined in `.harness/sensors/architecture-fitness.md`.

## Steps

Execute each check and report pass/fail:

### 1. Forbidden project references

```bash
grep -r "Tasks.Persistence" src/Tasks.Application/ --include="*.csproj"
grep -rE "Tasks\.(Application|Persistence|Api|DependencyInjection)" src/Tasks.Domain/ --include="*.csproj"
grep -r "Tasks.Persistence" src/Tasks.Api/ --include="*.csproj"
```

### 2. Forbidden namespace usage in Application layer

```bash
grep -r "ProjectionsDbContext" src/Tasks.Application/ --include="*.cs"
grep -r "EventsDbContext" src/Tasks.Application/ --include="*.cs"
grep -r "Tasks.Persistence" src/Tasks.Application/ --include="*.cs"
```

### 3. Result pattern on command handlers

```bash
grep -rL "Result<" src/Tasks.Application/UseCases/ --include="*Handler.cs"
```

## Output format

For each check, report:
- ✓ PASS — if no output (no violations found)
- ✗ FAIL — if matches found, list each file and line

At the end, print a summary:
```
Sensor results: N/3 passed
```

If any check fails, print the specific files and suggest the fix.
