# One-try

3D action / roguelite game project built with **Unity 6.3 LTS** (`6000.3.x`).

See [`GAME_DESIGN.md`](./GAME_DESIGN.md) for the design document and
[`docs/case-studies/issue-8/`](./docs/case-studies/issue-8/) for the
case study explaining the engine choice and migration from the initial
Unreal Engine prototype.

## Requirements

- [Unity Hub](https://unity.com/download)
- Unity Editor **6.3 LTS** (any 6000.3.x revision). The pinned revision
  is in `ProjectSettings/ProjectVersion.txt`; Unity Hub will offer to
  install the matching version when you first open the project.

## Opening the project

1. Install Unity Hub.
2. Install Unity Editor 6.3 LTS via Unity Hub.
3. In Unity Hub, click **Add → Add project from disk** and select this
   folder.
4. Open the project. The first open takes a few minutes while Unity
   imports packages and generates the local `Library/` cache.
5. Open `Assets/Scenes/SampleScene.unity`.

## Player Mannequin

A multi-part humanoid mannequin prefab lives at
`Assets/Characters/Player/PlayerMannequin.prefab`. To test it:

1. In an open scene, use the menu **GameObject → One-try → Add Player Mannequin**,
   or drag the prefab from the Project window.
2. Press **Play**. The mannequin plays a looping idle animation (gentle chest
   breathing).

The mannequin is built from Unity primitive Capsules arranged in a humanoid
hierarchy (clavicles, upper arms, forearms, hands; thighs, calves, feet).
No external assets required — everything is in the repo as readable YAML.
Segment meshes and materials can be swapped independently for future
SIGNALIS-style visuals.

## GitHub Actions — Portable Windows EXE

The workflow [`.github/workflows/build.yml`](./.github/workflows/build.yml)
packages a portable Windows EXE on every push. It uses the
[GameCI](https://game.ci) `unity-builder` action, runs on a free
GitHub-hosted Linux runner, cross-compiles to `StandaloneWindows64`, and
uploads the result as a workflow artifact.

### Setup (required once)

The build needs a Unity license. Personal licenses are free.

1. Create a free Unity account at <https://id.unity.com> if you don't
   have one.
2. Generate a Unity Personal license file (`.ulf`) — easiest path is to
   trigger this workflow once with no `UNITY_LICENSE` set; the
   "Request manual activation file" job will produce a `.alf` artifact,
   upload it to <https://license.unity3d.com/manual>, and download the
   resulting `.ulf`.
3. Add the following secrets in **Settings → Secrets and variables →
   Actions**:
   - `UNITY_LICENSE` — full text content of the `.ulf` file.
   - `UNITY_EMAIL` — your Unity account email.
   - `UNITY_PASSWORD` — your Unity account password.

Until those secrets are set, push and pull-request runs pass the
`Check for UNITY_LICENSE in GitHub Secrets` preflight but skip the Unity
packaging job, so no portable EXE artifact is uploaded. Manual
`workflow_dispatch` runs are allowed to continue to the activation job
so the owner can download the `.alf` activation-request artifact.

### Running the build

- Triggers automatically on push to `main` / `issue-*` branches and on
  PRs targeting `main`. Can also be run manually via **Actions → Build
  Portable Windows EXE → Run workflow**.
- After a successful run with Unity secrets configured, open the run
  summary and download the artifact named `OneTry-Win64-<run-number>`.
  The artifact contains `OneTry-Win64.zip`; extract it and run
  `StandaloneWindows64/OneTry.exe` — no installation required.
