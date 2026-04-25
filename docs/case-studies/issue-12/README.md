# Case study — Issue #12: GitHub Actions still produce no portable EXE

This folder collects the raw data and analysis requested in issue #12. The
issue reports that 2 of 3 GitHub Actions jobs did not run and there is still no
downloadable portable Windows EXE.

| File | Purpose |
|---|---|
| [issue.md](./issue.md) | Original issue text and scope. |
| [timeline.md](./timeline.md) | Reconstructed sequence of CI events from the issue and fresh run data. |
| [root-cause.md](./root-cause.md) | Root-cause analysis using the latest PR run. |
| [analysis.md](./analysis.md) | Technical analysis of the workflow behavior, artifact state, and evidence. |
| [proposed-solutions.md](./proposed-solutions.md) | Practical resolution paths and the selected recommendation. |
| [references.md](./references.md) | Repository, CI, GameCI, Unity, and GitHub references used. |
| [raw/](./raw/) | Downloaded issue, PR, workflow, CI metadata, logs, and artifacts JSON. |

## TL;DR

The latest PR run for branch `issue-12-996a8bae5eab` completed with overall
conclusion `success`, but that success did **not** mean a Windows build ran.

Fresh evidence from run
[`24940817291`](https://github.com/Jhon-Crow/One-try/actions/runs/24940817291):

| Job | Conclusion | Steps run |
|---|---:|---:|
| Check for `UNITY_LICENSE` in GitHub Secrets | `success` | 4 |
| Request Unity activation file | `skipped` | 0 |
| Package Windows Shipping Build (`StandaloneWindows64`) | `skipped` | 0 |

The run uploaded **zero artifacts**. The log states that `UNITY_LICENSE` is
empty and warns that no `OneTry-Win64` ZIP artifact will be uploaded.

## Root cause

The portable EXE is missing because the repository still lacks the Unity CI
secrets required by GameCI:

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

The workflow is intentionally configured to keep normal push/PR runs green
when the license is missing, while skipping the real build job. The manual
activation job only runs for `workflow_dispatch`, so it is also skipped on the
pull request event.

This is the same configuration blocker identified in the earlier issue #10
case study, now confirmed again with fresh issue #12 CI data.

## Required owner action

1. Run the `Build Portable Windows EXE` workflow manually from the Actions UI.
2. Download the `.alf` activation artifact produced by the activation job.
3. Exchange it for a `.ulf` license file at Unity's manual activation portal.
4. Add `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD` as repository
   Actions secrets.
5. Re-run the workflow and verify that `OneTry-Win64-<run-number>` appears in
   the run artifacts.
