# Issue #15 — всё ещё не собирается portable exe

## Original issue text

**Title:** всё ещё не собирается portable exe  
**Author:** Jhon-Crow  
**State:** OPEN  
**Created:** 2026-04-25  
**Number:** #15  
**URL:** https://github.com/Jhon-Crow/One-try/issues/15

### Body

> https://github.com/Jhon-Crow/One-try/actions/runs/24940978768
>
> Please collect data related about the issue to this repository, make sure we compile that data to `./docs/case-studies/issue-{id}` folder, and use it to do deep case study analysis (also make sure to search online for additional facts and data), and propose possible solutions (including known existing components/libraries, that solve similar problem or can help in solutions).
>
> сделай чтоб собирался portable exe и корректно отрбатывал весь ci cd

**Translation:** "Still won't build a portable exe — make it build a portable exe and have the entire CI/CD run correctly."

## Scope

Issue #15 is the third consecutive report of the same symptom: the
`Build Portable Windows EXE` workflow runs green but produces no downloadable
`OneTry-Win64` ZIP artifact containing `OneTry.exe`.

The issue owner explicitly referenced a specific run
(`24940978768`) and requested:
1. Data collection and case study analysis in `./docs/case-studies/issue-15/`.
2. A deep root-cause and solution investigation, including online research.
3. A fix so that the portable EXE actually builds and CI/CD "works correctly".

## Requirements

| # | Requirement | Type |
|---|---|---|
| R1 | `OneTry-Win64-<run-number>` artifact uploaded on each successful CI run | Functional |
| R2 | The artifact contains `StandaloneWindows64/OneTry.exe` | Functional |
| R3 | All three CI jobs either run or skip for documented reasons | CI |
| R4 | No red/failed status on normal push or PR runs | CI |
| R5 | Case study analysis recorded in `./docs/case-studies/issue-15/` | Documentation |

## History

| Issue | PR | Description |
|---|---|---|
| #5 | #6 | GameCI-based Unity build pipeline introduced. `.alf`/`.ulf` activation mechanism documented. |
| #8 | #9 | Unity migration from UE5. `ProjectVersion.txt` updated to Unity 6.3 LTS. |
| #10 | #11 | Case study: `build` job always skipped because `UNITY_LICENSE` secret is absent. |
| #12 | #13 | Case study confirmed same root cause again from fresh CI evidence. |
| **#15** | **#16** | **This issue.** Third report. Explicit request to fix CI/CD so it "works correctly". |
