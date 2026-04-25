# Root-cause analysis - Issue #17

## Problem statement

Issue #17 reports that the portable Windows EXE still is not built. The linked
CI run is successful, but there is no `OneTry-Win64.zip` artifact.

## Five whys

| # | Question | Answer |
|---|---|---|
| 1 | Why is there no portable EXE artifact? | The upload step is in the `Package Windows Shipping Build (StandaloneWindows64)` job, and that job was skipped. |
| 2 | Why was the build job skipped? | The workflow condition requires `needs.checklicense.outputs.has_credentials == 'true'`. |
| 3 | Why was `has_credentials` false? | The preflight job showed both `UNITY_EMAIL` and `UNITY_PASSWORD` were empty. |
| 4 | Why are those secrets empty? | Repository Actions secrets have not been configured by an owner or maintainer with access to the upstream repository settings. |
| 5 | Why did this keep recurring after prior fixes? | The workflow was intentionally warning-only when secrets were missing, so `main` could show a green workflow even when no release artifact was possible. |

## Root cause

The upstream repository still does not have the required Unity credential
secrets configured. The workflow correctly avoids attempting a licensed Unity
build without credentials, but it previously reported success for that state.
That status hid the fact that no portable EXE was produced.

## What is not the root cause

| Hypothesis | Evidence |
|---|---|
| The ZIP step failed | The ZIP step never ran because the build job was skipped. |
| `upload-artifact@v4` failed | The upload step never ran. |
| Unity compilation failed | No Unity build was attempted in the linked run. |
| The project cannot build for Windows | The linked evidence does not reach compilation, so this remains untested until credentials are configured. |
