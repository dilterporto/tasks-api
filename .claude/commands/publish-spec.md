Create a GitHub issue from an existing spec file.

## Input

The argument is the spec file path or issue number of an existing spec (e.g. `/publish-spec 13` or `/publish-spec .harness/specs/features/13-slug.md`).

## Steps

1. Locate the spec file:
   - If a number is given, find `.harness/specs/features/<N>-*.md`
   - If a path is given, read that file directly
   - If no spec is found, stop and tell the user to run `/spec <N>` first

2. Parse the spec file and extract:
   - **Title** — from the `# Spec: [Title]` heading
   - **Issue number** — from the `**Issue:** #N` field (if already set, skip creation and report the existing issue)
   - **Agent** — from `**Agent:**` field
   - **Label** — map agent to label:
     - `engineer` → `feature` (or `bug` if the title contains "fix" or "bug")
     - `infra-engineer` → `infrastructure`
   - **Context** → becomes the "Problem" section of the issue body
   - **What will be built** → becomes the "Requirements" section
   - **Out of Scope** → becomes the "Out of scope" section
   - **Acceptance Criteria** → appended as a checklist under Requirements

3. Create the GitHub issue:
   ```
   gh issue create --title "<Title>" --label "<label>" --body "<formatted body>"
   ```

   Body format:
   ```
   ## Problem
   <Context section from spec>

   ## Requirements
   <What will be built section>

   ### Acceptance Criteria
   - [ ] AC-1
   - [ ] AC-2

   ## Out of scope
   <Out of Scope section>
   ```

4. After creation, update the spec file's `**Issue:** #N` field with the new issue number if it was not set

5. Print the issue URL and the updated spec path
