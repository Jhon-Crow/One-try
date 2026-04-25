# Proposed solutions — Issue #12

## Recommended solution: complete Unity license setup

This is the only option that produces the requested portable EXE artifact.

### Steps

1. Open the workflow page:
   `https://github.com/Jhon-Crow/One-try/actions/workflows/build.yml`
2. Use **Run workflow** on `main` to trigger a manual `workflow_dispatch` run.
3. Because `UNITY_LICENSE` is missing, the `Request Unity activation file` job
   should run and upload a `.alf` artifact.
4. Download the `.alf` artifact.
5. Upload it to Unity's manual activation portal:
   `https://license.unity.com/manual`
6. Download the resulting `.ulf` license file.
7. In GitHub repository settings, add these Actions secrets:

| Secret | Value |
|---|---|
| `UNITY_LICENSE` | Full text content of the `.ulf` file |
| `UNITY_EMAIL` | Unity account email |
| `UNITY_PASSWORD` | Unity account password |

8. Re-run the workflow.
9. Verify that the build job runs and uploads `OneTry-Win64-<run-number>`.

## Alternative A: make missing secrets fail PR CI

Change the warning step into an explicit failure on push and pull request
events when `UNITY_LICENSE` is missing.

Pros:

- Impossible to overlook.
- CI status matches the fact that no portable EXE was produced.

Cons:

- Every PR remains red until the owner configures secrets.
- It still does not produce an EXE by itself.

This option is useful only if the project values a hard failing signal more
than green review status.

## Alternative B: keep current workflow but add stronger documentation

Add a top-of-README callout and perhaps a CI badge note that says no portable
EXE is produced until Unity secrets are configured.

Pros:

- Low risk.
- Keeps the current workflow behavior.
- Reduces confusion for future issue reports.

Cons:

- Still relies on the owner noticing and completing the secret setup.
- A green run with zero artifacts remains possible.

## Alternative C: publish artifacts through Releases after successful builds

After the Unity license is configured and normal artifacts work, add a release
upload path so builds are retained permanently instead of for the current
30-day artifact retention window.

Pros:

- Better distribution link for testers.
- Permanent downloads can be tied to tags.

Cons:

- Does not solve the current missing-license blocker.
- Adds release automation complexity.

## Selected path

Use the recommended solution first: complete Unity license setup. The workflow
already has the necessary build, ZIP, and artifact upload steps. The missing
piece is the repository secret configuration that only a maintainer can
complete.
