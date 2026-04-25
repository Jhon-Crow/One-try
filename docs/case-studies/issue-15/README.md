# Case study — Issue #15: всё ещё не собирается portable exe

This folder collects all raw data, logs, and analysis for issue #15, which is
the **third consecutive report** that the portable Windows EXE is still not
being produced by GitHub Actions.

| File | Purpose |
|---|---|
| [issue.md](./issue.md) | Original issue text, requirements table, history of prior issues. |
| [timeline.md](./timeline.md) | Full sequence of CI events from PR #6 through the issue #15 branch run. |
| [root-cause.md](./root-cause.md) | Five-whys analysis including new evidence on Unity 6.x activation issues. |
| [analysis.md](./analysis.md) | Deep technical analysis: workflow state machine, Unity 6.x `.alf`/`.ulf` problems, credential-based alternatives. |
| [proposed-solutions.md](./proposed-solutions.md) | Five solutions ranked by feasibility; selected solution implemented. |
| [references.md](./references.md) | All consulted sources (GameCI, Unity, GitHub Actions, community reports). |
| [raw/](./raw/) | Raw data: issue JSON, PR JSON, CI run list, log snapshots, workflow snapshot. |

## TL;DR

Issue #15 ("всё ещё не собирается portable exe" — "still won't build a
portable exe") is the third consecutive report of the same symptom. Every CI
run reports `success` but zero artifacts are uploaded.

### Root cause (short version)

The `build` job is permanently skipped because no Unity credentials are set
in the repository's Actions secrets. The documented `.alf`→`.ulf` activation
path has additional friction: Unity deprecated manual Personal license
activation for Unity 6.x, making the portal `license.unity3d.com/manual`
unreliable for Personal accounts — which explains why three issues have been
filed without the owner completing the activation.

### What changed in this PR

The workflow (`.github/workflows/build.yml`) was updated to:

1. **Replace the `UNITY_LICENSE` gate** with a check for `UNITY_EMAIL` and
   `UNITY_PASSWORD` (the two secrets already familiar to the owner).
2. **Add `buildalon/activate-unity-license@v1`** before the build step —
   this action handles credential-based Personal license activation at runtime,
   with no `.alf`/`.ulf` exchange needed.
3. **Add a license return step** (`if: always()`) to free the activation slot
   after each build.
4. **Remove the `activation` job** (the `.alf` generation job) since it is no
   longer needed.

`README.md` was updated to document the new two-secret setup.

### Required owner action

Add two repository secrets in **Settings → Secrets and variables → Actions**:

| Secret | Value |
|---|---|
| `UNITY_EMAIL` | Unity account email address |
| `UNITY_PASSWORD` | Unity account password |

After adding these secrets, push any commit or re-run the workflow. The
`build` job will run, activate a Personal license, compile
`StandaloneWindows64`, create `OneTry-Win64.zip`, and upload
`OneTry-Win64-<run-number>` as a downloadable artifact.

### Validation

The issue is resolved when a workflow run shows **all** of:

1. `Check for Unity credentials in GitHub Secrets` logs that credentials are found.
2. `Package Windows Shipping Build (StandaloneWindows64)` runs (not skipped).
3. The `Activate Unity license` step succeeds.
4. The `Build Unity project` step (`unity-builder@v4`) succeeds.
5. `OneTry-Win64.zip` is created and uploaded.
