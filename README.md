# One-try

Empty 3D scene project built with Unreal Engine 5.

## Requirements

- Unreal Engine 5.3 (download from [Epic Games Launcher](https://www.unrealengine.com/))

## Opening the project

1. Install Unreal Engine 5.3 via Epic Games Launcher.
2. Double-click `OneTry.uproject` to open in the editor.

## GitHub Actions — Portable Windows EXE

The workflow `.github/workflows/build.yml` packages a portable Windows EXE on every push.

### Setup (required once)

The build uses Epic's official Docker image (`ghcr.io/epicgames/unreal-engine`), which requires:

1. Link your GitHub account to an Epic Games account:
   - Go to [unrealengine.com](https://www.unrealengine.com/) → account → **Connected Accounts** → connect GitHub.
   - Accept the `EpicGames/UnrealEngine` repository invitation (sent via email).
2. Create a GitHub Personal Access Token (classic) with the `read:packages` scope.
3. Add it as a repository secret named **`EPIC_GITHUB_TOKEN`**:
   - Repository → **Settings** → **Secrets and variables** → **Actions** → **New repository secret**.

### Running the build

- Trigger automatically on push/PR, or manually via **Actions** → **Build Portable Windows EXE** → **Run workflow**.
- Download the ZIP artifact from the workflow run summary. Extract and run `OneTry.exe` — no installation required.

### Alternative: Self-hosted Windows runner

If you have a Windows machine with UE5 installed, follow the comments in `build.yml` to switch to a self-hosted runner (no Epic GitHub token needed).
