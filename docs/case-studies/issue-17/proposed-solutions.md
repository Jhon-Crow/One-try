# Proposed solutions - Issue #17

## Solution A - Fail release-relevant runs when credentials are missing (selected)

Change the preflight job so missing Unity credentials are an error for:

- manual `workflow_dispatch` runs;
- pushes to `main`.

Keep PR and issue-branch runs warning-only because those contexts often do not
have access to repository secrets.

**Pros**

- Stops `main` from looking releasable when no EXE can be built.
- Preserves contributor feedback on PRs.
- Requires no fake or insecure credential workaround.

**Cons**

- Still requires the owner to add Unity credentials before the artifact exists.

## Solution B - Always fail when credentials are missing

Make every run fail unless `UNITY_EMAIL` and `UNITY_PASSWORD` are present.

**Pros**

- Strongest signal.

**Cons**

- Fork PRs and prepared issue branches would fail even though they cannot access
  the missing upstream secrets. This creates noise without producing an EXE.

## Solution C - Keep warning-only behavior

Leave the workflow green when credentials are absent.

**Pros**

- No noisy CI failures.

**Cons**

- This is the current failure mode: users repeatedly see successful CI with no
  portable executable artifact.

## Solution D - Store a Unity license file instead of credentials

Use `UNITY_LICENSE` instead of `UNITY_EMAIL` and `UNITY_PASSWORD`.

**Pros**

- Common pattern for some GameCI workflows.

**Cons**

- Prior issues show the manual `.alf` to `.ulf` path is high-friction for this
  repository. It also requires storing a long license XML secret and refreshing
  it when Unity licensing changes.

## Solution E - Use a self-hosted runner with Unity already activated

Run the packaging job on a machine controlled by the maintainer.

**Pros**

- Can avoid repeated hosted-runner activation.

**Cons**

- Higher operational burden and security maintenance than GitHub-hosted runners
  for a small Unity project.
