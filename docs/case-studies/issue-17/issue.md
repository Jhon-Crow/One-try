# Issue #17

## Original title

`vse eshche ne sobiraetsya portable exe`

Russian meaning: "the portable exe still does not build".

## Original body

The issue links to this workflow job:

<https://github.com/Jhon-Crow/One-try/actions/runs/24941217038/job/73035169259?pr=16#step:3:8>

It asks to:

1. collect repository data related to the issue;
2. compile that data in `./docs/case-studies/issue-17`;
3. perform a deep case-study analysis;
4. search online for additional facts and data;
5. propose possible solutions, including existing components or libraries that
   solve similar problems or help with solutions;
6. fix the issue.

## Acceptance criteria used for this PR

The repository cannot produce a Unity Windows build without valid Unity account
credentials. The practical fix is therefore:

1. preserve raw CI evidence proving why no artifact was built;
2. document the root cause and owner action clearly;
3. make missing credentials fail for `main` and manual release builds instead
   of allowing a green status with no EXE;
4. keep PR and issue-branch runs non-blocking when repository secrets are not
   available;
5. align the activation action with the current documented major version.
