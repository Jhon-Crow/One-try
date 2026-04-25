# Proposed Solutions — Issue #8

This document lists the recommended Unity packages, OSS components, and
the migration plan that PR #9 implements.

## 1. Engine choice

Use **Unity 6.3 LTS** (`6000.3.x`).

- LTS, supported through December 2027.
- Aligns with `GAME_DESIGN.md` ("Движок: Unity").
- Compatible with the GameCI `unity-builder@v4` action.

The pinned editor version is recorded in
`ProjectSettings/ProjectVersion.txt`. GameCI's `unity-builder` reads this
file by default (`unityVersion: auto`) so changing the Unity version in
one place updates CI too.

## 2. Recommended Unity packages (initial `Packages/manifest.json`)

The shipped manifest is intentionally minimal. It includes:

| Package | Why |
|---|---|
| `com.unity.inputsystem` | Modern Input System — required for the input buffering / dash / parry windows / hold-vs-tap distinctions described in `GAME_DESIGN.md` § "Атаки". |
| `com.unity.cinemachine` | Procedural cameras for arena combat and boss encounters. |
| `com.unity.render-pipelines.universal` (URP) | Reasonable default 3D pipeline for the dark/contrast art direction in § "Визуальный стиль". HDRP is overkill for a stylized dark-fantasy roguelite and slows iteration. |
| `com.unity.textmeshpro` | UI/HUD text. |
| `com.unity.timeline` | Boss intros and cutscene-style finishers. |
| `com.unity.test-framework` | Required by GameCI's optional test step and useful for unit-testing the input/combat state machines. |
| `com.unity.ide.rider`, `com.unity.ide.visualstudio`, `com.unity.ide.vscode` | Editor integrations — GameCI templates ship them; harmless and convenient. |
| Standard `com.unity.modules.*` | Engine modules required by URP, audio, animation, particles, physics, etc. |

These are added once at the project level so subsequent feature PRs
(player model #5, abstract Unity classes #7) can rely on them without
re-installing.

## 3. Existing components that solve design-doc requirements

The design doc asks for several mechanics that already have well-tested
OSS or first-party implementations to draw from. Listed here so future
PRs can adopt rather than reinvent:

| Mechanic in `GAME_DESIGN.md` | Existing reference / library |
|---|---|
| Input buffering, action mapping | `Unity-Technologies/InputSystem` samples (`PlayerInput`, action assets). |
| Coyote time, jump buffer | Pattern documented in Unity Discussions; trivial to implement against `CharacterController` or Rigidbody. |
| Dash with i-frames | Common Unity pattern using coroutines + a transient invulnerability flag. Reference: Code Monkey "Dash" tutorials, `2D-Platformer-Hunter` GitHub project (mechanic translates to 3D). |
| Parry window | State-machine pattern; can be modeled with `UnityEvent`s on the character's combat controller. |
| Boss state machines | `Unity-Technologies/EntityComponentSystemSamples` and the `Animator` state machines / `PlayableGraph` for cutscene-grade transitions. |
| Camera / arena framing | `Cinemachine` `CinemachineFreeLook` for player camera, `CinemachineTargetGroup` for boss arenas. |
| HUD (stamina, energy from § "Ресурсы") | UI Toolkit (built-in) with TMP. |

None of these are integrated by PR #9 — PR #9 only sets the **foundation**
that lets these be added in follow-up work.

## 4. CI build approach

Replace `.github/workflows/build.yml` with a workflow built on
[`game-ci/unity-builder@v4`](https://github.com/game-ci/unity-builder).

Why GameCI:

- Free and OSS.
- Most-used Unity CI integration on GitHub.
- Supports `StandaloneWindows64` from a Linux runner via Docker, so we
  don't need a Windows runner.
- Configuration matches what the previous UE5 workflow targeted: a
  zipped portable Windows build uploaded as a workflow artifact.

Required repo secrets (replacing the previous `EPIC_GITHUB_TOKEN`):

- `UNITY_LICENSE` — full content of the Unity Personal `.ulf` license file
  (free, generated via `game-ci/unity-request-activation-file` once).
- `UNITY_EMAIL` — Unity account email.
- `UNITY_PASSWORD` — Unity account password.

The workflow:

1. Caches the `Library/` folder per platform (massive build speed-up).
2. Runs `game-ci/unity-builder@v4` with `targetPlatform: StandaloneWindows64`.
3. Zips `build/StandaloneWindows64/` as `OneTry-Win64.zip`.
4. Uploads it as a workflow artifact (`actions/upload-artifact@v4`).
5. Triggers on push to `main` and `issue-*` branches, on PRs to `main`,
   and on manual dispatch — same trigger surface as the previous UE5
   workflow.

A graceful no-op path is included for the case where `UNITY_LICENSE` is
not yet configured: the build job is skipped with a clear message instead
of failing red. This avoids breaking CI for new contributors / forks.

## 5. Project layout after migration

```
.
├── .github/workflows/build.yml       # Unity build (GameCI)
├── .gitignore                        # Unity-targeted ignore list
├── Assets/                           # Unity assets
│   ├── Scenes/SampleScene.unity
│   └── README.md                     # placeholder until first feature PR
├── Packages/
│   └── manifest.json                 # pinned package set
├── ProjectSettings/
│   └── ProjectVersion.txt            # pinned editor version
├── docs/
│   └── case-studies/issue-8/         # this case study
├── GAME_DESIGN.md                    # unchanged — source of truth
├── README.md                         # rewritten for Unity workflow
```

Removed:

- `OneTry.uproject`
- `Config/Default*.ini`
- `Content/` (UE5 maps directory)
- `.gitkeep` files that were UE5-specific

## 6. What this PR does **not** do

Out of scope (left for follow-up issues / PRs):

- Implementing the player controller (issue #5).
- Defining abstract Unity classes for combat / boss state machines
  (issue #7).
- Authoring scenes, art, animations, audio.
- Adding code that depends on the editor having opened the project once
  (e.g. fully-formed `.asset` files, generated `.meta` GUIDs for new
  assets).

The skeleton is deliberately minimal so Unity itself fills in any
missing default `ProjectSettings/*.asset` files on first open. This is
the same pattern used by `unity-actions-example` and other GameCI
template repos.
