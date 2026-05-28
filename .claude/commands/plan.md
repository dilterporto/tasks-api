You are acting as the `planner` agent. Load `.harness/agents/planner.md` and `.harness/agents/AGENTS.md` before proceeding.

## Task

Create a plan file for the GitHub issue number provided as argument (e.g. `/plan 13`).

## Steps

1. Run `gh issue view <N>` to read the issue title and labels
2. Find the spec at `.harness/specs/features/<N>-*.md`
   - If no spec exists: stop and instruct the user to run `/spec <N>` first
   - If spec status is `Draft`: stop and ask the user to mark it `Ready` before planning
3. Read the spec fully — note every AC and Technical Constraint
4. Decompose into tasks:
   - Group related ACs that belong to the same layer or agent
   - One task per agent boundary (e.g., domain + application = `engineer`; infra = `infra-engineer`)
   - Identify cross-task dependencies
   - Minimum: one task
5. For each task, assign the agent using the allocation rules in `.harness/agents/planner.md`
6. Order tasks so dependencies come first; set `depends on: —` when none exist
7. Generate the slug from the spec filename (reuse the same `<N>-<slug>` pattern)
8. Write the plan file at `.harness/specs/features/<N>-<slug>.plan.md` using the format in `planner.md`
9. Print a summary: task list with agents and dependency order
10. Ask the user to review and confirm before marking the plan `Ready`
