# Timeline / Sequence of events

Reconstructed from `git log --all --oneline`, the GitHub issue/PR comment timestamps, and commit-level diffs, in response to the owner's PR #6 ask: *"compile that data … and use it to do deep case study analysis … in which we will reconstruct timeline/sequence of events, find root causes of the problem"*.

| When (UTC) | What | Where | Notes |
|---|---|---|---|
| earlier | Initial commit | `40ae0aa` | Empty repository. |
| earlier | Add game design document for One-try | `40a2893` (merged via PR #3 / branch `issue-2-…`) | `GAME_DESIGN.md` explicitly states `Движок: **Unity**`. |
| earlier | "Add empty UE5 3D scene project and GitHub Actions portable EXE build" | `fd9254a` (merged via PR #4 / branch `issue-1-…`) | First time **Unreal Engine 5** appears. The Unreal choice was made by an automated agent on issue #1, **not** by the project owner. The CI workflow is built around Epic's Docker image (`ghcr.io/epicgames/unreal-engine`) and requires the secret `EPIC_GITHUB_TOKEN`. |
| 2026-04-25 | Issue #5 opened by **@Jhon-Crow** | https://github.com/Jhon-Crow/One-try/issues/5 | "добавь модельку игрока…". The issue does not specify an engine. |
| 2026-04-25 19:56–20:02 | Branch `issue-5-d34229c636e3` created with the player mannequin **on Unreal** | commit `1efd896` | At this point the agent assumed Unreal because `OneTry.uproject` was already in the tree. Implemented as `Tools/Setup_PlayerMannequin.py` materialising Epic's `SKM_Quinn_Simple` mannequin and `MF_Idle` animation. |
| 2026-04-25 20:04 | PR #6 opened, marked ready | konard | https://github.com/Jhon-Crow/One-try/pull/6 |
| 2026-04-25 20:05 | First CI run on PR #6 fails | `Build Portable Windows EXE` workflow | The Epic Docker pull fails because the repo owner has not configured `EPIC_GITHUB_TOKEN`. Same failure was already present on `main` after PR #4 merged — i.e. the EXE build had **never worked**. |
| 2026-04-25 20:16 | **Owner comment** on PR #6 by @Jhon-Crow | comment `4320470708` | "это должно быть на движке UNITY и сейчас почему то нет сборки portable exe в github actions". Owner asks for Unity + a working portable EXE build, and for the case study to reconstruct the timeline. |
| 2026-04-25 20:17+ | Auto-restart triggered. Two parallel sessions reworked the branch: a Unity 6.3 LTS project skeleton + GameCI workflow lands first, then this case-study addition documents the timeline + root cause | branch `issue-5-d34229c636e3` (commits `695088d`, `3cef194`, `0247caf`, `f7ca748`, `45ece90`) | The migration is split: PR #9 / issue #8 lands the Unity engine baseline; PR #6 / issue #5 lands the multi-part mannequin and idle animation on top of it. |

## Visual timeline (engine choice over time)

```
[Initial commit]
      │
      ▼
[GAME_DESIGN.md says “Движок: Unity”]   ← owner intent
      │
      ▼
[issue #1 → PR #4 lands an Unreal scaffold]   ← engine drift introduced here
      │
      ▼
[issue #5 → PR #6 builds Unreal mannequin on top of the drift]
      │
      ▼
[owner PR #6 comment: “это должно быть на движке UNITY”]   ← drift detected
      │
      ▼
[issue #8 → PR #9 + this branch: project rebased onto Unity 6.3 LTS]   ← drift corrected
      │
      ▼
[PR #6: multi-part Unity mannequin + idle animation]   ← this case study
```

## Why the previous EXE build never produced an EXE

1. The `Build Portable Windows EXE` workflow added in PR #4 called `RunUAT.sh` inside `ghcr.io/epicgames/unreal-engine:dev-slim-5.3.2`.
2. That image is **gated** behind Epic's GitHub Container Registry — pulling it requires the puller to be a member of the `EpicGames` GitHub org **and** to authenticate with a Personal Access Token (the workflow expected it as `EPIC_GITHUB_TOKEN`).
3. The repo owner never configured that secret (and configuring it is a one-time, account-level chore that lives outside the codebase). As a result every workflow run failed on `Pull container` long before any build step ran — there had never been an `OneTry.exe` artifact.

After the Unity migration the build pipeline runs on stock `ubuntu-latest` with [GameCI `unity-builder@v4`](https://game.ci/docs/github/builder/) and only needs three secrets that anyone with a free Unity Personal account can produce in ~3 minutes (`UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD`). See [`solutions.md`](../issue-8/solutions.md) for the full migration plan and [README.md](../../../README.md) for the secret-setup runbook.
