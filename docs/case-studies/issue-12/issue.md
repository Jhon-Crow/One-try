# Issue #12 — Original report and scope

## Original issue

| Field | Value |
|---|---|
| Issue | [Jhon-Crow/One-try#12](https://github.com/Jhon-Crow/One-try/issues/12) |
| Title | `fix` |
| Created | 2026-04-25T21:13:03Z |
| Updated | 2026-04-25T21:13:17Z |
| State | open |
| Comments | none at collection time |

Original body:

```text
2 из 3 github actions не отработали, всё ещё нет собранного portable exe
Please download all logs and data related about the issue to this repository,
make sure we compile that data to `./docs/case-studies/issue-{id}` folder,
and use it to do deep case study analysis (also make sure to search online
for additional facts and data), in which we will reconstruct timeline/sequence
of events, find root causes of the problem, and propose possible solutions.
```

English summary:

```text
2 of 3 GitHub Actions did not run; there is still no built portable EXE.
Download all related logs and data, compile them under
docs/case-studies/issue-12, analyze the timeline, identify root causes, and
propose possible solutions.
```

## Scope of this case study

This case study covers:

- the latest CI run on branch `issue-12-996a8bae5eab`;
- the PR attached to the branch, [PR #13](https://github.com/Jhon-Crow/One-try/pull/13);
- the current workflow definition, `.github/workflows/build.yml`;
- downloaded CI logs and run metadata;
- the artifact state for the latest run;
- related facts from GameCI, Unity license activation, and GitHub Actions
  behavior.

It does not change gameplay, Unity assets, or build workflow code because the
observed blocker is a repository secret configuration issue.
