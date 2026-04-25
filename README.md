# One-try

3D action / roguelite game project built with **Godot 4.3** (free and open-source).

See [`GAME_DESIGN.md`](./GAME_DESIGN.md) for the design document.

## Requirements

- [Godot Engine 4.3](https://godotengine.org/download) — free, no account required

## Opening the project

1. Download and install Godot 4.3 from <https://godotengine.org/download>.
2. Launch Godot, click **Import**, and select this folder (`project.godot`).
3. Press **F5** (or the Play button) to run the main scene (`scenes/main.tscn`).

## Player Mannequin

A multi-part humanoid mannequin scene lives at `scenes/player_mannequin.tscn`.
To add it to a scene, drag it from the FileSystem dock into the viewport, then
press **F5** — the mannequin plays a looping idle animation (gentle chest
breathing). The mannequin is built from Godot `BoxMesh` primitives arranged in
a humanoid hierarchy (clavicles, upper arms, forearms, hands; thighs, calves,
feet). No external assets required — everything is in the repo as readable text.
Segment meshes and materials can be swapped independently for future
SIGNALIS-style visuals.

## GitHub Actions — Portable Windows EXE

The workflow [`.github/workflows/build.yml`](./.github/workflows/build.yml)
packages a portable Windows EXE on every push. It uses the official Godot
headless binary, runs on a free GitHub-hosted Linux runner, and uploads the
result as a workflow artifact.

**No credentials or secrets are required.** Godot is free and open-source —
the workflow downloads Godot and export templates automatically.

### Running the build

- Triggers automatically on push to `main` / `issue-*` branches and on
  PRs targeting `main`. Can also be run manually via
  **Actions → Build Portable Windows EXE → Run workflow**.
- After a successful run, open the run summary and download the artifact
  named `OneTry-Win64-<run-number>`. Extract the ZIP and run `OneTry.exe` —
  no installation required.
