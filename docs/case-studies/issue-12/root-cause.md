# Root cause — Issue #12

## Problem statement

The user expected a portable Windows EXE artifact, but the latest GitHub
Actions run produced no artifact. In the run UI, 2 of 3 jobs did not run.

## Direct cause

The `Package Windows Shipping Build (StandaloneWindows64)` job was skipped.
That job is the only job that can create `OneTry-Win64.zip` and upload the
`OneTry-Win64-<run-number>` artifact.

The job condition in `.github/workflows/build.yml` is:

```yaml
if: needs.checklicense.outputs.is_unity_license_set == 'true'
```

For run `24940817291`, the `checklicense` job found:

```text
UNITY_LICENSE:
UNITY_LICENSE is NOT set.
```

So the output was `false`, and the build job never started.

## Five whys

1. **Why is there no portable EXE?**  
   Because no `OneTry-Win64` artifact was uploaded.

2. **Why was no artifact uploaded?**  
   Because the build job, which contains the ZIP and upload steps, was skipped.

3. **Why was the build job skipped?**  
   Because `needs.checklicense.outputs.is_unity_license_set` was `false`.

4. **Why was that output false?**  
   Because the repository Actions secret `UNITY_LICENSE` was empty or missing.

5. **Why has this persisted across issues?**  
   Because repository secrets must be added manually by an owner or maintainer
   with repository settings access. The workflow can detect and explain the
   missing configuration, but it cannot create the Unity license secret itself.

## Root cause

The root cause is missing Unity build credentials in GitHub Actions secrets:

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

This is a repository configuration gap, not a Unity project compilation error
and not a broken artifact upload step.

## Contributing factor

The workflow is designed so push and pull request runs remain green when the
Unity license is missing. It emits a warning but does not fail the workflow.
That makes the status less disruptive for code review, but it can be
misinterpreted as "the Windows build succeeded."

In reality, the green run only means the preflight check succeeded and the
workflow handled the missing secret as configured.
