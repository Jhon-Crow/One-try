# Technical analysis - Issue #17

## Workflow state machine

The workflow has two relevant jobs:

1. `checklicense` reads `secrets.UNITY_EMAIL` and `secrets.UNITY_PASSWORD`.
2. `build` runs only when `checklicense` outputs `has_credentials=true`.

In run `24941217038`, the preflight job logged both variables as empty:

```text
UNITY_EMAIL:
UNITY_PASSWORD:
Unity credentials (UNITY_EMAIL and/or UNITY_PASSWORD) are NOT set.
```

The following warning then explains the user-visible symptom:

```text
Unity credentials are missing, so the Windows packaging job is skipped and no OneTry-Win64 ZIP artifact is uploaded for this run.
```

The raw run JSON confirms the downstream packaging job conclusion is `skipped`.

## Why a code-only fix cannot create the EXE

Unity Editor builds require a valid Unity license activation. For GitHub-hosted
CI, that activation requires credentials or a license file stored in repository
secrets. Pull requests from forks and regular issue branches cannot invent or
read missing upstream secrets. Therefore the repository owner must configure
`UNITY_EMAIL` and `UNITY_PASSWORD` before any CI workflow can produce a real
Windows build.

## What the workflow can fix

The workflow can make the missing artifact state impossible to miss:

- PR and issue-branch runs can remain warning-only so normal code review is not
  blocked by missing upstream secrets.
- `main` push and manual `workflow_dispatch` runs should fail when credentials
  are missing, because those runs are the release paths where a green status
  implies a portable executable should exist.

This PR implements that distinction.

## Activation action version

The existing workflow used `buildalon/activate-unity-license@v1`. Current
Buildalon documentation shows `buildalon/activate-unity-license@v2` for
credential-based Unity license activation. Updating both activation and return
steps to `v2` reduces the chance of hitting old action behavior after the
secrets are configured.
