# Deep analysis — Issue #12

## 1. Current repository state

| Area | State | Evidence |
|---|---|---|
| Engine | Unity 6.3 LTS family | `ProjectSettings/ProjectVersion.txt` |
| Workflow | GameCI-based Windows build | `.github/workflows/build.yml` |
| Target platform | `StandaloneWindows64` | `raw/build.yml.snapshot` |
| Artifact name pattern | `OneTry-Win64-<run-number>` | `raw/build.yml.snapshot` |
| Latest issue #12 run | `24940817291` | `raw/ci-run-24940817291.json` |
| Latest issue #12 artifact count | `0` | `raw/ci-run-24940817291-artifacts.json` |

## 2. Workflow structure

The workflow has three jobs:

| Job | Purpose | Runs when |
|---|---|---|
| `checklicense` | Detect whether `UNITY_LICENSE` exists and expose `is_unity_license_set`. | Always |
| `activation` | Request a Unity `.alf` activation file. | Missing license and manual `workflow_dispatch` only |
| `build` | Build `StandaloneWindows64`, zip it, and upload the portable artifact. | `UNITY_LICENSE` is set |

For normal pull request events without a Unity license, only the first job
runs. The other two jobs are skipped by design.

## 3. Latest CI evidence

Run:
[`24940817291`](https://github.com/Jhon-Crow/One-try/actions/runs/24940817291)

| Field | Value |
|---|---|
| Event | `pull_request` |
| Created | 2026-04-25T21:14:00Z |
| Head SHA | `d9c0309c74d4d9465d08fa1ae4478aae5eba7c8c` |
| Overall conclusion | `success` |
| Artifact count | `0` |

Job conclusions:

| Job | Conclusion | Steps |
|---|---:|---:|
| Check for `UNITY_LICENSE` in GitHub Secrets | `success` | 4 |
| Request Unity activation file | `skipped` | 0 |
| Package Windows Shipping Build (`StandaloneWindows64`) | `skipped` | 0 |

Important log lines from `raw/ci-log-run-24940817291.log`:

- line 43: `UNITY_LICENSE:` is empty;
- line 45: `UNITY_LICENSE is NOT set.`;
- line 46: configure `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD`;
- line 55: warning that no `OneTry-Win64` ZIP artifact is uploaded.

## 4. Why the activation job did not run

The activation job condition is:

```yaml
if: needs.checklicense.outputs.is_unity_license_set == 'false' && github.event_name == 'workflow_dispatch'
```

Issue #12's latest run was a `pull_request` event, not a manual
`workflow_dispatch` event. Therefore the activation job correctly skipped.

This is intentional. Activation creates a machine-specific `.alf` file that a
repository owner must exchange for a Unity `.ulf` license and store as a
secret. It is not useful to run on every push or pull request.

## 5. Why the workflow can be green while no EXE exists

The only executed job completed successfully. The two skipped jobs did not
fail the workflow. GitHub Actions therefore reported the workflow run as
successful even though it produced no portable build.

This is a status-design tradeoff:

- Green PR status avoids blocking branches before the owner completes Unity
  license setup.
- The warning explains the missing artifact.
- The downside is that a green run is easy to confuse with a successful
  Windows package build.

## 6. Online source cross-check

GameCI's activation documentation states that Unity installations used by CI
must be activated and describes requesting an activation file for GitHub
Actions. GameCI's builder documentation covers the Unity builder action used
by this workflow.

Unity's manual activation documentation describes the `.alf` request and
`.ulf` license-file flow. This matches the repository's existing README setup
instructions and confirms that the missing step is owner-side license
activation, not a code change.

## 7. Validation criteria

The issue is resolved only when a workflow run shows all of the following:

1. `Check for UNITY_LICENSE in GitHub Secrets` logs that `UNITY_LICENSE` is
   set.
2. `Package Windows Shipping Build (StandaloneWindows64)` runs instead of
   skipping.
3. The `Build Unity project` step succeeds.
4. `OneTry-Win64.zip` is created.
5. The run uploads an artifact named `OneTry-Win64-<run-number>`.

Until then, any green run with zero artifacts should be treated as a preflight
success, not a portable EXE build success.
