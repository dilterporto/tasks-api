Create and checkout a branch for the GitHub issue number provided as argument (e.g. `/branch 17`).

## Steps

1. Run `gh issue view <N>` to read the issue title and labels
2. Determine the branch prefix from the issue labels:
   - `bug` → `fix`
   - `feature`, `enhancement`, `domain`, `persistence`, `api`, `caching`, `observability` → `feat`
   - `tech-debt`, `documentation`, `infra` → `chore`
   - default → `feat`
3. Generate the slug from the issue title: lowercase, replace spaces with hyphens, remove special characters, max 40 chars
4. Build the branch name: `<prefix>/#<N>-<slug>`
5. Check if the branch already exists locally or on origin:
   - If it exists: run `git checkout <branch>` and inform the user
   - If not: run `git checkout -b <branch>`
6. Print the branch name and confirm the user is now on it
7. Remind: "Run `/plan <N>` if you haven't planned this issue yet, then `/implement <N>` to start."
