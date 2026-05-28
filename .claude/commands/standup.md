Show the current state of the project: open issues, open PRs, and active branches.

## Steps

Run these commands in parallel and format the output as a standup report:

1. `gh issue list --state open --limit 20 --json number,title,labels,assignees`
2. `gh pr list --state open --limit 10 --json number,title,headRefName,author,isDraft`
3. `git branch -r --sort=-committerdate | grep -v "HEAD\|main" | head -10`

## Output format

```
## Standup — <today's date>

### Open Issues (<N>)
- #N [label] Title

### Open PRs (<N>)
- #N Title (branch) [DRAFT if applicable]

### Active branches
- branch-name
```

Group issues by label. Flag any PR that has been open for more than 2 days (trunk-based development: branches should be short-lived). Flag any issue without a spec file in `.harness/specs/features/`.
