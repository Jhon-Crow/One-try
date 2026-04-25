# Root-cause analysis — Issue #15

## Problem statement

Issue #15 is the **third consecutive** report that the portable Windows EXE
still is not being produced. All CI runs show `success` yet zero artifacts are
uploaded. The issue owner explicitly asks for the EXE to be built and for
CI/CD to "work correctly".

---

## Five-whys analysis

### Why is there still no `OneTry-Win64` ZIP artifact?

| # | Question | Answer |
|---|---|---|
| 1 | Why is no artifact uploaded? | The `build` job is **skipped** on every run; the upload-artifact step is inside that job and never executes. |
| 2 | Why is the `build` job skipped? | Its condition is `if: needs.checklicense.outputs.is_unity_license_set == 'true'`. The `checklicense` job always outputs `is_unity_license_set=false`. |
| 3 | Why does `checklicense` always output `false`? | The check `if [ -n "$UNITY_LICENSE" ]` tests whether the `UNITY_LICENSE` environment variable is non-empty. It is always empty because the `UNITY_LICENSE` repository secret has never been configured. |
| 4 | Why is `UNITY_LICENSE` never configured? | Obtaining the secret requires the repository owner to complete a one-time Unity Personal license activation: generate an `.alf` activation request file, exchange it for a `.ulf` license at Unity's activation portal, and paste the `.ulf` content into the secret. This is a manual, owner-only operation. |
| 5 | Why has the owner not completed activation after three issues? | The workflow intentionally keeps overall CI green even when the license is absent (to avoid red PRs on forks). This "polite" design removes urgency. Additionally, the Unity Personal license `.alf`→`.ulf` exchange process has known friction points (deprecated portal, platform-specific instructions, periodic re-activation). |

**Root cause:** The repository's three Unity CI secrets (`UNITY_LICENSE`,
`UNITY_EMAIL`, `UNITY_PASSWORD`) have never been set. Without them, the
`build` job is permanently skipped and no EXE is ever produced.

---

## Compounding factors — new evidence for issue #15

### CF-1 — Repeated issues signal friction in the activation path

Issues #10, #12, and now #15 all share the same root cause. The fact that
three consecutive issues were filed without the owner completing activation
strongly suggests that:

1. The activation instructions are present in `README.md` but are not being
   followed.
2. There may be a technical or practical barrier preventing the owner from
   completing the Unity Personal license activation.

### CF-2 — Unity's manual activation path has become unreliable

Online research (Unity Discussions, GameCI issue tracker) reveals that Unity
deprecated the legacy licensing module for newer Unity versions (2022.1+). The
manual `.alf` → `.ulf` flow via `https://license.unity3d.com/manual` does
**not** work reliably for Personal licenses on Unity 6.x.

Community reports confirm: "Unity no longer supports manual activation of
Personal licenses" — the portal sometimes refuses to issue a `.ulf` for
Personal accounts, requiring workarounds.

Since `game-ci/unity-request-activation-file@v2` (used by the `activation`
job in the workflow) generates the `.alf` file, and exchanging that `.alf` for
a `.ulf` is now unreliable for Unity 6.x Personal licenses, the documented
activation path in `README.md` may not work at all even if the owner tries it.

### CF-3 — Credential-based activation is a better fit for Unity 6

The GameCI `unity-builder@v4` (used in the `build` job) supports direct
credential-based activation via `UNITY_EMAIL` and `UNITY_PASSWORD` without
requiring a pre-generated `UNITY_LICENSE`. When only `UNITY_EMAIL` and
`UNITY_PASSWORD` are provided and `UNITY_LICENSE` is absent, the builder can
attempt to activate and fetch the license directly from Unity's servers at
runtime.

However, the current workflow's `checklicense` job blocks the `build` job
entirely when `UNITY_LICENSE` is absent, preventing even this fallback path
from being tried.

### CF-4 — Alternative activation actions exist

Several actively maintained GitHub Actions bypass the `.alf`/`.ulf` mechanism
entirely:

- `buildalon/activate-unity-license@v2` — credential-based, Personal + Pro,
  tested on Unity 6.x runners.
- `game-ci/unity-license-activate` — CLI-based activation via email/password.
- `mob-sakai/unity-activate` — lightweight credential-based activation.

None of these require the owner to generate and manually exchange activation
files.

---

## What is NOT the root cause

| Hypothesis | Evidence against |
|---|---|
| The workflow YAML has a logic bug | Workflow runs without error; the skip logic is intentional and correctly implemented. |
| GameCI `unity-builder@v4` is broken | When license is present, the action correctly builds StandaloneWindows64 from Linux. |
| The Unity project itself cannot be built | No build has ever been attempted; the project structure (Assets/, Packages/, ProjectSettings/) is normal. |
| The ZIP/upload steps are broken | `upload-artifact@v4` is first-party; the issue is that these steps are never reached. |
| GitHub Actions is failing for unrelated reasons | All job conclusions are `success` or `skipped` — no infrastructure errors. |

---

## Summary

The portable EXE is missing entirely because of absent Unity CI secrets. The
current `.alf`→`.ulf` activation path in the workflow is unreliable for Unity
6.x Personal licenses. The fix requires either:

(a) Completing the activation with an alternative credential-based method, or  
(b) Updating the workflow to use a credential-based activation action that does
not depend on the `.alf`/`.ulf` exchange.
