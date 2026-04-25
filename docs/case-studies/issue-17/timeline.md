# Timeline - Issue #17

| Time (UTC) | Event | Evidence | Outcome |
|---|---|---|---|
| 2026-04-25T22:06 | PR #16 merged | Recent PR metadata | Workflow had been changed to credential-based Unity activation. |
| 2026-04-25T21:35 | Run `24941217038` created | Issue #17 link | Credential preflight ran, but both Unity credential secrets were empty. |
| 2026-04-25T21:35 | Job `73035169259` step 2 | Raw CI log | `UNITY_EMAIL:` and `UNITY_PASSWORD:` are blank in the job environment. |
| 2026-04-25T21:35 | Job `73035169259` step 3 | Raw CI log | Warning reports no `OneTry-Win64` ZIP artifact will be uploaded. |
| 2026-04-25T21:35 | Packaging job | Raw run JSON | `Package Windows Shipping Build (StandaloneWindows64)` is skipped. |
| 2026-04-25T22:08 | Current PR #18 initial run | `ci-runs.json` | Branch run is green, but it only validates the non-secret PR path. |

## Key observation

The linked job did not fail because the workflow intentionally treated missing
credentials as a warning. That made the Actions page look successful even
though the only job capable of producing the portable EXE was skipped.
