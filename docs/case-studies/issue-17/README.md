# Case study - Issue #17: vse eshche ne sobiraetsya portable exe

This folder collects the raw CI evidence and analysis for issue #17, the next
report that the GitHub Actions workflow still does not produce a portable
Windows executable.

| File | Purpose |
|---|---|
| [issue.md](./issue.md) | Original issue text and acceptance criteria. |
| [timeline.md](./timeline.md) | Relevant CI and PR history for the current report. |
| [root-cause.md](./root-cause.md) | Five-whys analysis of the missing artifact. |
| [analysis.md](./analysis.md) | Technical analysis of workflow behavior and secret handling. |
| [proposed-solutions.md](./proposed-solutions.md) | Options considered and selected remediation. |
| [references.md](./references.md) | Online and repository references used for the analysis. |
| [raw/](./raw/) | Raw issue, PR, workflow, and CI log snapshots. |

## TL;DR

The CI run linked from issue #17 is green, but it does not build an EXE. The
preflight job prints that `UNITY_EMAIL` and `UNITY_PASSWORD` are empty, then
the Windows packaging job is skipped. Therefore no `OneTry-Win64.zip` artifact
can be uploaded.

## What changed

The workflow was updated so missing Unity credentials are no longer silent for
release-relevant runs:

1. `workflow_dispatch` and `main` push runs now fail during the credential
   preflight when `UNITY_EMAIL` or `UNITY_PASSWORD` is missing.
2. Pull request and issue-branch push runs still report a warning and skip the
   build when secrets are unavailable, because forks and PRs cannot reliably
   access repository secrets.
3. `buildalon/activate-unity-license` was bumped from `v1` to `v2`, matching
   current Buildalon documentation for credential-based activation.

## Required owner action

Add these repository secrets in **Settings -> Secrets and variables -> Actions**:

| Secret | Value |
|---|---|
| `UNITY_EMAIL` | Unity account email address |
| `UNITY_PASSWORD` | Unity account password |

After those secrets are set, run **Actions -> Build Portable Windows EXE -> Run
workflow**. The build is resolved only when the `Package Windows Shipping Build
(StandaloneWindows64)` job runs and uploads the `OneTry-Win64-<run-number>`
artifact.
