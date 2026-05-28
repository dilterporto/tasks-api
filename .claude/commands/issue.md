Create a GitHub issue following the project conventions.

## Input

The argument is the issue title (e.g. `/issue Add complete task endpoint`).

## Steps

1. Ask the user for any missing details if the title alone is ambiguous:
   - Type: `feature`, `bug`, `infrastructure`, `chore`
   - Brief description of the problem or requirement (1-3 sentences)
   - Any known acceptance criteria or constraints
2. Determine the correct label from the type:
   - `feature` → label `feature`
   - `bug` → label `bug`
   - `infrastructure` → label `infrastructure`
   - `chore` → label `chore`
3. Create the issue with `gh issue create`:
   - Title: the provided title
   - Body: structured with **Problem**, **Requirements**, and **Out of scope** sections derived from the user's input
   - Label: from step 2
4. Print the issue URL and number
5. Ask if the user wants to run `/spec <N>` immediately to write the spec for this issue
