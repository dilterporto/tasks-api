You are acting as the `architect` agent. Load `.harness/agents/architect.md` and `.harness/architecture/overview.md` before proceeding.

## Task

Create a spec file for the GitHub issue number provided as argument (e.g. `/spec 13`).

## Steps

1. Run `gh issue view <N>` to read the issue title, body, and labels
2. Determine the agent type from the label:
   - `feature` or `bug` → `engineer`
   - `infrastructure` → `infra-engineer`
3. Generate a slug from the issue title (lowercase, hyphens, max 5 words)
4. Create the file `.harness/specs/features/<N>-<slug>.md` using `.harness/specs/_template.md` as the base structure
5. Fill in:
   - **Issue:** #N
   - **Author:** from `gh issue view` assignee, or `@dilterporto` if unassigned
   - **Status:** `Draft`
   - **Agent:** based on label (step 2)
   - **Context:** derived from the issue body — why this exists, what problem it solves
   - **What will be built:** clear behavioral description (not implementation)
   - **Inputs and outputs:** inferred from the issue or left as placeholders if not specified
   - **Behavior:** Must/Must not/Should statements derived from the issue requirements
   - **Acceptance Criteria:** one AC per requirement in the issue, each independently testable
   - **Technical Constraints:** layer boundaries, patterns, what is out of bounds
   - **Out of Scope:** anything the issue explicitly excludes or that is obviously out of bounds
   - **Sensors:** test names to verify each AC (use placeholder names if not yet known)
6. After creating the file, print the path and a summary of the ACs written
7. Ask the user to review and confirm before marking the spec `Ready`
