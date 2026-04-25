# Timeline — Issue #12

All times are UTC.

| Time | Event | Evidence |
|---|---|---|
| 2026-04-25T21:13:03Z | Issue #12 opened. The report says 2 of 3 GitHub Actions jobs did not run and there is still no portable EXE. | `raw/issue-12.json` |
| 2026-04-25T21:14:00Z | PR workflow run `24940817291` starts for branch `issue-12-996a8bae5eab`, commit `d9c0309c74d4d9465d08fa1ae4478aae5eba7c8c`. | `raw/ci-runs.json`, `raw/ci-run-24940817291.json` |
| 2026-04-25T21:14:02Z | `Check for UNITY_LICENSE in GitHub Secrets` starts. | `raw/ci-log-run-24940817291.log` |
| 2026-04-25T21:14:03Z | The check finds `UNITY_LICENSE` empty. | `raw/ci-log-run-24940817291.log` lines 43-47 |
| 2026-04-25T21:14:03Z | The workflow emits a warning that the Windows packaging job is skipped and no `OneTry-Win64` ZIP artifact is uploaded. | `raw/ci-log-run-24940817291.log` lines 48-56 |
| 2026-04-25T21:14:04Z | The license check job completes successfully. | `raw/ci-run-24940817291.json` |
| 2026-04-25T21:14:05Z | `Request Unity activation file` is skipped. This is expected on `pull_request`; the job only runs for manual `workflow_dispatch` when the license is missing. | `raw/ci-run-24940817291.json`, `raw/build.yml.snapshot` |
| 2026-04-25T21:14:05Z | `Package Windows Shipping Build (StandaloneWindows64)` is skipped because `is_unity_license_set` is `false`. | `raw/ci-run-24940817291.json`, `raw/build.yml.snapshot` |
| 2026-04-25T21:14:05Z | Overall workflow conclusion is `success`, but artifact count is `0`. | `raw/ci-run-24940817291.json`, `raw/ci-run-24940817291-artifacts.json` |

## Sequence reconstruction

The workflow did exactly what the YAML describes:

1. A pull request event triggered the build workflow.
2. `checklicense` tested whether `secrets.UNITY_LICENSE` is set.
3. The secret was empty, so the job output became
   `is_unity_license_set=false`.
4. The `activation` job did not run because the event was not
   `workflow_dispatch`.
5. The `build` job did not run because it requires
   `is_unity_license_set == 'true'`.
6. GitHub marked skipped jobs as skipped and the completed workflow as
   successful because the only executed job succeeded.
7. No build directory was produced and no portable ZIP artifact was uploaded.
