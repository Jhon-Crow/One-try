# Root-cause analysis

Two problems were flagged by the owner on PR #6:

1. **The project is on Unreal Engine, but it should be on Unity.**
2. **There is no portable EXE artifact coming out of GitHub Actions.**
3. **After switching to Unity, the workflow still reported success while the packaging job was skipped, so there was still no artifact to download.**

Both have the same upstream cause; the EXE failure is a symptom of the engine-choice failure.

## Problem 1 — wrong engine

### Five whys

| # | Question | Answer |
|---|---|---|
| 1 | Why was the player mannequin built on Unreal? | Because `OneTry.uproject`, `Config/DefaultEngine.ini`, and a UE-targeted `build.yml` were already in the repo when issue #5 was opened. |
| 2 | Why were those Unreal files in the repo? | They were added in PR #4 (commit `fd9254a`) while resolving issue #1 ("Add empty 3D scene project + portable EXE build"). |
| 3 | Why did issue #1 produce an Unreal scaffold instead of a Unity one? | Issue #1 did **not** name an engine. The implementing agent picked Unreal without consulting the existing `GAME_DESIGN.md`, which already declared `Движок: Unity`. |
| 4 | Why didn't anyone notice the mismatch when PR #4 merged? | `GAME_DESIGN.md` was added by a separate, parallel issue (#2 / PR #3) merged at almost the same time. Neither PR cross-checked the other. |
| 5 | Why didn't issue #5's implementation re-check the engine choice? | The implementing agent inherited PR #4's Unreal scaffold as ground truth and built on top of it instead of re-deriving the engine from `GAME_DESIGN.md`. |

**Root cause:** **issue #1 was implemented without reading `GAME_DESIGN.md`**, and downstream issues compounded the drift instead of catching it.

### What the current branch does about it

- Removes the Unreal scaffold (`OneTry.uproject`, `Config/Default*.ini`, `Content/`, `Tools/Setup_PlayerMannequin.py`, the UE variant of `docs/PlayerModel.md`) — landed via PR #9 (issue #8).
- Adds a Unity 6.3 LTS project skeleton (`Assets/`, `Packages/manifest.json`, `ProjectSettings/`).
- Reimplements the player mannequin **as a Unity prefab** that satisfies the same R1–R6 requirements (multi-part rig, idle animation, no binary blobs) — text-only YAML.

### Preventing recurrence

- This case-study folder ships **alongside** the implementation, not as an after-the-fact retrospective, so future engine-checking is impossible to skip when reviewing.
- `README.md` now opens with the engine + design-doc reference, so any future scaffold-change PR has to update those lines too.

## Problem 2 — no portable EXE artifact

### Five whys

| # | Question | Answer |
|---|---|---|
| 1 | Why is there no portable EXE artifact in any workflow run? | Every workflow run failed before the build step even started. |
| 2 | Why did the run fail before the build? | The `container:` block could not pull `ghcr.io/epicgames/unreal-engine:dev-slim-5.3.2`. |
| 3 | Why couldn't it pull the image? | The image lives behind the `EpicGames` private GHCR org and requires authenticating with the secret `EPIC_GITHUB_TOKEN`. |
| 4 | Why was `EPIC_GITHUB_TOKEN` not set? | Configuring it requires the **owner's** GitHub account to be linked to an Epic Games account that has accepted the UE EULA, then to mint a PAT and store it as a repo secret — three manual steps the owner never performed. |
| 5 | Why was a build pipeline that requires owner-side manual setup chosen in the first place? | Because PR #4 was implemented as if any Unreal-on-CI guide on the internet would just work, without checking whether the owner could actually meet the prerequisites. |

**Root cause:** **the previous build pipeline depended on a manual EULA-gated step the owner never opted into**, so it could never produce an artifact.

### What the current branch does about it

- Replaces the Epic Docker workflow with [GameCI `unity-builder@v4`](https://game.ci/docs/github/builder/), which:
  - runs on stock `ubuntu-latest`,
  - needs only three secrets (`UNITY_EMAIL`, `UNITY_PASSWORD`, `UNITY_LICENSE`) that anyone with a free Unity Personal account can produce in ~3 minutes following the [game-ci activation guide](https://game.ci/docs/github/activation/),
  - supports `targetPlatform: StandaloneWindows64` natively, producing `OneTry.exe` and the loose data folder,
  - pairs naturally with `actions/upload-artifact@v4` to publish a portable ZIP.
- Adds a `checklicense` job that **skips the build cleanly** when `UNITY_LICENSE` is not set, so first-time forks see a green-with-a-message workflow instead of a red one. The on-demand `activation` job produces the manual-activation file the owner needs upload-once.
- Documents the secret setup in both `README.md` and the workflow file's leading comments. Caches `Library/` so subsequent runs are fast.

### Follow-up after owner feedback

The first Unity workflow draft made `checklicense` pass and skipped the real
`build` job when `UNITY_LICENSE` was missing. That was still wrong for this
issue because GitHub displayed a successful workflow run with **zero
artifacts**. The corrected workflow now:

- fails push and pull-request runs when Unity secrets are missing, because
  those runs cannot satisfy the "portable EXE artifact" requirement;
- keeps manual `workflow_dispatch` activation available without failing, so
  the owner can download the `.alf` activation request artifact;
- uploads the final EXE as artifact `OneTry-Win64-<run-number>` after secrets
  are configured. Inside it, `OneTry-Win64.zip` contains
  `StandaloneWindows64/OneTry.exe`.

### Preventing recurrence

- Workflows that need owner-configured secrets now declare those secrets in a **Setup (required once)** block visible at the top of `README.md`, so the secret-setup task is impossible to miss when reviewing the PR.
- The build workflow uploads artifact-friendly logs on failure so the next investigation has artifacts to read instead of staring at a red square in the Actions UI.
