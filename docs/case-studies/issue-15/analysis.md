# Deep analysis — Issue #15

## 1. Repository state

| Area | State | Source |
|---|---|---|
| Engine | Unity 6.3 LTS | `ProjectSettings/ProjectVersion.txt` |
| Build target | `StandaloneWindows64` | `.github/workflows/build.yml` |
| CI framework | GameCI `unity-builder@v4` | `.github/workflows/build.yml` |
| Activation job action | `game-ci/unity-request-activation-file@v2` | `.github/workflows/build.yml` |
| Artifact name pattern | `OneTry-Win64-<run-number>` | `.github/workflows/build.yml` |
| Latest referenced run | `24940978768` (main) | Issue #15 body |
| Branch PR run | `24941016116` (issue-15 branch) | CI run list |
| Artifact count (all runs) | **0** | Every run since PR #6 merged |

## 2. Workflow state machine

```
push/PR event
     │
     ▼
┌─────────────────────────────────┐
│  checklicense job               │
│  check: UNITY_LICENSE non-empty │
│                                 │
│  result: is_unity_license_set   │
│  = false  (always, no secret)   │
└──────────┬──────────────────────┘
           │
    ┌──────┴───────┐
    ▼              ▼
[if false +    [if true]
 dispatch]     ─────────────────────────────
    │           Package Windows build job
    ▼           (unity-builder@v4)
activation      → zip → upload-artifact
job             ─────────────────────────────
(skipped on        [NEVER REACHED]
 push/PR)
```

For every `push` and `pull_request` event:
1. `checklicense` runs and finds `UNITY_LICENSE` empty → outputs `false`.
2. `build` job is gated on `is_unity_license_set == 'true'` → **skipped**.
3. `activation` job is gated on `is_unity_license_set == 'false' && event == workflow_dispatch` → **skipped** (wrong event).
4. Overall run conclusion: `success` (only one job ran, and it succeeded).
5. Artifacts: **0**.

## 3. Evidence from issue-15 CI run `24941016116`

| Field | Value |
|---|---|
| Event | `pull_request` |
| Branch | `issue-15-c1168914bce6` |
| Created | 2026-04-25T21:24:25Z |
| Conclusion | `success` |
| Artifacts | `0` |

Key log lines:
- `UNITY_LICENSE:` ← empty
- `UNITY_LICENSE is NOT set.`
- `Configure UNITY_LICENSE, UNITY_EMAIL, UNITY_PASSWORD secrets to enable Windows packaging.`
- `##[warning]UNITY_LICENSE is missing, so the Windows packaging job is skipped and no OneTry-Win64 ZIP artifact is uploaded for this run.`

This is identical to the evidence in the issue #10 and issue #12 case studies.

## 4. The `.alf`→`.ulf` activation path for Unity 6.x

The `activation` job uses `game-ci/unity-request-activation-file@v2`, which
launches Unity in batch mode with `-createManualActivationFile` to generate
an `.alf` (Activation License File). The owner is then supposed to upload the
`.alf` to `https://license.unity3d.com/manual` and download a `.ulf` license.

**Issue for Unity 6.x Personal licenses:**

Unity deprecated the legacy licensing module starting with Unity 2022.1. The
`license.unity3d.com/manual` portal no longer reliably issues `.ulf` files for
Personal-tier accounts. Multiple community reports on Unity Discussions confirm:

> "Unity no longer supports manual activation of Personal licenses"
> — Unity Discussions thread `926760`, Aug 2023 onwards, still active in 2025–2026

GameCI's own troubleshooting documentation acknowledges that the Personal
activation flow requires workarounds (e.g., forcing the "Personal" option to
be visible via browser developer tools on the legacy portal).

Since the repository uses Unity 6.3 LTS, the `.alf`→`.ulf` path is at
significant risk of failing even if the owner attempts it. This explains why
three issues have been filed without resolution.

## 5. Credential-based activation alternatives

Several GitHub Actions implement direct credential-based Unity activation,
bypassing the `.alf`/`.ulf` entirely:

### 5a. `buildalon/activate-unity-license@v2`

```yaml
- uses: buildalon/activate-unity-license@v2
  with:
    license: 'Personal'
    username: ${{ secrets.UNITY_USERNAME }}
    password: ${{ secrets.UNITY_PASSWORD }}
```

- Authenticates directly against Unity's licensing servers at runtime.
- Supports Personal and Professional licenses.
- No `.alf`/`.ulf` exchange needed.
- Tested with Unity 6.x on ubuntu-latest runners.
- Published on GitHub Marketplace; actively maintained as of 2025–2026.

### 5b. `game-ci/unity-license-activate` (CLI tool)

```yaml
- uses: game-ci/unity-license-activate@v2
  with:
    UNITY_USERNAME: ${{ secrets.UNITY_EMAIL }}
    UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
    UNITY_SERIAL: ${{ secrets.UNITY_SERIAL }}  # Pro only; omit for Personal
```

- GameCI's own CLI-based activation.
- For Personal licenses, serial is not required.
- Requires only email + password secrets.

### 5c. `game-ci/unity-builder@v4` with direct credentials

`unity-builder@v4` already accepts `UNITY_EMAIL` and `UNITY_PASSWORD`. When
`UNITY_LICENSE` is absent but credentials are provided, the builder attempts
headless activation at build time. GameCI release notes from July 2024 state:
"Workaround for manual personal license activation is not needed anymore with
`game-ci/unity-builder@v4`."

## 6. Recommended workflow change

The current `checklicense` gate blocks the `build` job even when credentials
(`UNITY_EMAIL`, `UNITY_PASSWORD`) are available. The gate should be
restructured so that:

1. If `UNITY_LICENSE` **or** credentials are set → allow the build to run.
2. The `build` job handles activation via `buildalon/activate-unity-license@v2`
   before calling `unity-builder@v4`.
3. The `checklicense` job becomes a preflight reporter rather than a hard gate.

This change reduces the secrets required from the owner to just:
- `UNITY_EMAIL` (Unity account email)
- `UNITY_PASSWORD` (Unity account password)

No `.alf`/`.ulf` generation or exchange is needed.

## 7. Validation criteria

The issue is resolved when a workflow run shows **all** of the following:

1. `Check for license / credentials in GitHub Secrets` logs that credentials
   are found.
2. `Package Windows Shipping Build (StandaloneWindows64)` runs instead of
   skipping.
3. The `Build Unity project` step (or `unity-builder@v4`) completes without
   error.
4. `OneTry-Win64.zip` is created.
5. The run uploads an artifact named `OneTry-Win64-<run-number>` with at
   least one file matching `StandaloneWindows64/OneTry.exe`.

Until then, any green run with zero artifacts is a preflight success, not
a portable EXE build success.
