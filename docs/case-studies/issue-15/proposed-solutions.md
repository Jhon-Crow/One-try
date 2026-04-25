# Proposed solutions — Issue #15

## Context

The portable EXE is not produced because `UNITY_LICENSE` is never set and the
`build` job is gated on that secret. Three consecutive issues (#10, #12, #15)
confirm the secret has never been configured. The `.alf`→`.ulf` activation
path documented in `README.md` is unreliable for Unity 6.x Personal licenses.

This document proposes practical solutions ranked by feasibility.

---

## Solution A — Credential-based workflow with `buildalon/activate-unity-license` (Recommended)

**What changes:** The workflow is updated to:
1. Replace the `checklicense` binary gate with a check for credentials
   (`UNITY_EMAIL` / `UNITY_PASSWORD`).
2. Add an activation step using `buildalon/activate-unity-license@v2` before
   calling `unity-builder@v4`.
3. Add a license return step after the build to free the activation slot.

**Required owner action:** Add two repository secrets:
- `UNITY_EMAIL` — Unity account email address
- `UNITY_PASSWORD` — Unity account password

No `.alf` file generation, no Unity website portal, no `.ulf` exchange.

**Workflow snippet:**

```yaml
- name: Activate Unity license
  uses: buildalon/activate-unity-license@v2
  with:
    license: 'Personal'
    username: ${{ secrets.UNITY_EMAIL }}
    password: ${{ secrets.UNITY_PASSWORD }}

- name: Build Unity project
  uses: game-ci/unity-builder@v4
  with:
    targetPlatform: StandaloneWindows64
    buildName: ${{ env.PROJECT_NAME }}

- name: Return Unity license
  if: always()
  uses: buildalon/activate-unity-license@v2
  with:
    return-license: true
```

**Pros:**
- No `.alf`/`.ulf` exchange; works with Unity 6.x Personal licenses.
- Well-documented, actively maintained action on GitHub Marketplace.
- Credential-based: same two secrets already used by `unity-builder@v4`.
- Compatible with existing `game-ci/unity-builder@v4`.

**Cons:**
- Still requires owner to add `UNITY_EMAIL` and `UNITY_PASSWORD` secrets.
- Personal licenses expire periodically; re-activation may be needed.
- A Unity account is required (free).

**Effort:** Medium — workflow YAML update + owner adds 2 secrets.

---

## Solution B — Remove the `UNITY_LICENSE` gate; rely on `unity-builder@v4` direct credentials

**What changes:** The `checklicense` condition `is_unity_license_set == 'true'`
is replaced with a check for `UNITY_EMAIL` being non-empty. `unity-builder@v4`
handles activation itself when `UNITY_EMAIL` + `UNITY_PASSWORD` are provided.

GameCI release notes (July 2024): "Workaround for manual personal license
activation is not needed anymore with `game-ci/unity-builder@v4`."

**Required owner action:** Add `UNITY_EMAIL` and `UNITY_PASSWORD` secrets.

**Pros:**
- Minimal code change — only the gate condition changes.
- No new actions introduced.

**Cons:**
- Depends on GameCI's undocumented credential-based activation behaviour.
- Harder to debug if activation fails inside the builder step.
- May still require `UNITY_LICENSE` for some Unity versions.

**Effort:** Low — single-line condition change + 2 secrets.

---

## Solution C — Complete the existing `.alf`→`.ulf` activation process manually

**What changes:** No code change. Owner completes the documented activation:
1. Trigger a `workflow_dispatch` run (Actions → Run workflow) without any
   secrets set → `activation` job runs and uploads a `.alf` artifact.
2. Download the `.alf` from the run artifacts.
3. Visit `https://license.unity3d.com/manual` and exchange `.alf` for `.ulf`.
4. Copy full `.ulf` XML content into `UNITY_LICENSE` secret.
5. Add `UNITY_EMAIL` and `UNITY_PASSWORD` secrets.
6. Re-run the workflow.

**Pros:**
- No code change required.
- Uses the existing documented path.

**Cons:**
- The Unity manual activation portal is known to be unreliable for Personal
  licenses with Unity 6.x. The `.ulf` may not be issued.
- This exact path has been proposed in issues #10 and #12 without resolution.
- Requires 3 secrets instead of 2 (Solution A/B).

**Effort:** Low code effort, but high practical friction due to Unity portal
issues.

---

## Solution D — Use `game-ci/unity-license-activate` (CLI tool)

**What changes:** Add `game-ci/unity-license-activate` step to the `build` job
before calling `unity-builder@v4`. This CLI tool authenticates against Unity
servers using email + password.

```yaml
- name: Activate Unity License
  uses: game-ci/unity-license-activate@v2
  with:
    UNITY_USERNAME: ${{ secrets.UNITY_EMAIL }}
    UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
```

**Required owner action:** Add `UNITY_EMAIL` and `UNITY_PASSWORD` secrets.

**Pros:**
- Official GameCI tool.
- Credential-based, no `.alf`/`.ulf` needed.

**Cons:**
- Requires a separate return-license step.
- Less battle-tested with Unity 6.x than `buildalon/activate-unity-license`.

**Effort:** Medium.

---

## Solution E — Add stronger CI status signalling without fixing the root cause

**What changes:** The `checklicense` step is changed to fail (`exit 1`) on
push/PR events when `UNITY_LICENSE` is missing. PRs will show a red status.

**Pros:**
- Makes the problem unmissable.

**Cons:**
- Does not produce the portable EXE.
- Breaks all PR CI until the owner acts.

**Effort:** Trivial.

---

## Selected solution

**Solution A** is recommended: update the workflow to use
`buildalon/activate-unity-license@v2` for credential-based activation.

Rationale:
- Directly solves the root cause (never-produced EXE).
- Avoids the unreliable `.alf`→`.ulf` Unity portal process.
- Minimises owner action to adding two well-known secrets.
- Compatible with Unity 6.x and tested on ubuntu-latest.

**Implementation plan:**
1. Update `.github/workflows/build.yml`:
   - Change `checklicense` job to check for `UNITY_EMAIL` being non-empty
     (in addition to or instead of `UNITY_LICENSE`).
   - Add `buildalon/activate-unity-license@v2` step before `unity-builder@v4`.
   - Add return-license step with `if: always()`.
2. Update `README.md` Setup section to replace the `.alf`/`.ulf` instructions
   with the two-secret credential-based setup.
3. Owner adds `UNITY_EMAIL` and `UNITY_PASSWORD` as repository secrets.
4. Re-run the workflow to verify the build job runs and uploads an artifact.

---

## Known existing components / libraries

| Name | URL | Role |
|---|---|---|
| `buildalon/activate-unity-license` | https://github.com/buildalon/activate-unity-license | Credential-based activation, Unity 6.x tested |
| `game-ci/unity-license-activate` | https://github.com/game-ci/unity-license-activate | CLI activation tool |
| `game-ci/unity-builder` | https://github.com/game-ci/unity-builder | Cross-platform Unity build action |
| `game-ci/unity-request-activation-file` | https://github.com/game-ci/unity-request-activation-file | Generates `.alf` file (legacy, unreliable for Unity 6) |
| `game-ci/unity-activate` | https://github.com/game-ci/unity-activate | Activate Personal or Pro license |
| `mob-sakai/unity-activate` | https://github.com/mob-sakai/unity-activate | Lightweight credential-based activation |
| `actions/upload-artifact@v4` | https://github.com/actions/upload-artifact | Upload EXE ZIP artifact |
| `actions/cache@v4` | https://github.com/actions/cache | Cache Unity Library for faster builds |
