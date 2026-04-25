# Timeline — Issue #15

## Full historical sequence (issues #5 → #15)

| Date | Event | SHA / Run | Outcome |
|---|---|---|---|
| 2026-04-25T20:32 | PR #6 first CI runs (issue-5 branch) | `d71e6e1` | ✓ success (build skipped, license absent) |
| 2026-04-25T20:34 | More issue-5 runs — warning-only mode | `0ef7768` | ✓ success (build skipped) |
| 2026-04-25T20:43–45 | issue-5 runs with `exit 1` in checklicense | `e916857`…`bfcd41e` | ✗ failure (hard fail on missing license) |
| 2026-04-25T20:49–50 | Revert `exit 1` — warning mode restored | `a08c962`, `5a22edc` | ✓ success (build skipped) |
| 2026-04-25T20:54 | PR #6 merged to `main` | `f62c94a` | ✓ success on main |
| 2026-04-25T21:00 | issue-10 branch created; PR #11 CI run | `8ee5860` | ✓ success (build skipped) |
| 2026-04-25T21:09 | PR #11 CI run (updated commit) | `d5a3ee5` | ✓ success (build skipped) |
| 2026-04-25T21:11 | PR #11 merged to `main` | `bb2e6cd` | ✓ success on main |
| 2026-04-25T21:14 | issue-12 branch PR #13 CI run | `d9c0309` | ✓ success (build skipped, 0 artifacts) |
| 2026-04-25T21:18 | PR #13 CI run (updated commit) | `4e1a7d2` | ✓ success (build skipped) |
| 2026-04-25T21:21 | PR #13 CI run — post-merge | `284aa2f` | ✓ success (build skipped) |
| 2026-04-25T21:22 | PR #13 merged to `main` | `f41cb22` | ✓ success on main |
| 2026-04-25T21:22 | Issue #15 filed; run `24940978768` referenced | — | — |
| 2026-04-25T21:24 | issue-15 branch (PR #16) initial CI run | `7c63850` | ✓ success (build skipped, 0 artifacts) |

## Key observation

Across **every** run since the workflow was first introduced in PR #6, the
`Package Windows Shipping Build (StandaloneWindows64)` job was **skipped**.
The license check step always found `UNITY_LICENSE` empty. The overall
workflow always reported `success`.

No portable EXE artifact has ever been produced by any CI run in this
repository's history.

## Referenced CI run (issue #15)

Run `24940978768` (referenced in the issue body):
- Branch: `main`
- Event: `push`
- Created: 2026-04-25T21:22:27Z
- Jobs:
  - `Check for UNITY_LICENSE in GitHub Secrets` → **success** (4 steps)
  - `Package Windows Shipping Build (StandaloneWindows64)` → **skipped**
  - `Request Unity activation file` → **skipped**
- Artifacts: **0**

Current issue-15 branch run `24941016116`:
- Branch: `issue-15-c1168914bce6`
- Event: `pull_request`
- Created: 2026-04-25T21:24:25Z
- Same job pattern → 0 artifacts
