You are acting as the `engineer` agent. Load `.harness/agents/engineer.md`, `.harness/architecture/overview.md`, and `CLAUDE.md` before proceeding.

## Task

Implement the GitHub issue number provided as argument (e.g. `/implement 13`).

## Steps

1. Run `gh issue view <N>` to read the issue title, body, and labels
2. Check if a spec exists at `.harness/specs/features/<N>-*.md`
   - If no spec exists: stop and instruct the user to run `/spec <N>` first
   - If the spec status is `Draft`: stop and ask the user to mark it `Ready` before implementing
3. Read the spec file fully
4. Determine which guide to load based on the spec's scope:
   - New use case → `.harness/guides/use-case.md`
   - New domain event → `.harness/guides/domain-events.md`
   - Infrastructure → stop and delegate to `infra-engineer` via `/implement` is not applicable
5. Create the feature branch: `git checkout -b feat/#<N>-<slug>` (use the same slug as the spec filename)
6. Implement all acceptance criteria from the spec, following:
   - The dependency rules in `overview.md`
   - The patterns in the loaded guide
   - The constraints section of the spec
7. Write tests covering each AC — use `.harness/guides/testing.md` for conventions
8. Run `dotnet build Tasks.sln` — fix any build errors before continuing
9. Run `dotnet test tests/Tasks.Tests/` — fix any test failures before continuing
10. Run the architecture sensor checks from `.harness/sensors/architecture-fitness.md`
11. Update `CLAUDE.md` use case table if a new use case was added
12. Update the spec status to `Implementing`
13. Report which ACs are implemented and which tests cover them
