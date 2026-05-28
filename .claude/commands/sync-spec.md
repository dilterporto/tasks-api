Synchronize a spec file with its GitHub issue — update the issue body from the spec, or update the spec from the issue.

## Input

The argument is an issue number or spec path (e.g. `/sync-spec 13`).

## Steps

1. Locate both the spec and the issue:
   - Find the spec at `.harness/specs/features/<N>-*.md`
   - Fetch the current issue with `gh issue view <N> --json number,title,body,state,labels`

2. If the issue is **closed**, report it and ask whether to reopen or just update the spec status to `Done`

3. Compare spec vs issue and report differences:
   - Title mismatch
   - Acceptance criteria added to the spec but not in the issue body
   - Issue body has requirements not reflected in the spec

4. Ask the user which direction to sync:
   - **spec → issue**: update the GitHub issue body from the spec (preserves issue comments and metadata)
   - **issue → spec**: update the spec Context/What will be built sections from the issue body

5. Execute the chosen sync:
   - For **spec → issue**: run `gh issue edit <N> --body "<new body>"` using the same format as `/publish-spec`
   - For **issue → spec**: edit the spec file, updating the relevant sections

6. Report what changed
