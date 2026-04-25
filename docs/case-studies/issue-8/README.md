# Case Study — Issue #8

> **Issue:** [Jhon-Crow/One-try#8 — изучи документация и адаптируй под движок unity](https://github.com/Jhon-Crow/One-try/issues/8)
> **Pull request:** [Jhon-Crow/One-try#9](https://github.com/Jhon-Crow/One-try/pull/9)
> **Branch:** `issue-8-02cf259d291f`

This folder contains the data and analysis used to drive the implementation
in PR #9.

## Files

| File | Purpose |
|---|---|
| `analysis.md` | Deep case-study analysis: problem framing, root cause of the engine mismatch, and tradeoffs of UE5 vs. Unity for the One-try design. |
| `solutions.md` | Concrete proposed solutions, recommended Unity packages and OSS components, and the migration plan that is implemented in PR #9. |
| `raw/` | Raw data snapshots collected before changes were made (issue body, comments, repo file snapshots, related PR/issue lists). |

## TL;DR

`GAME_DESIGN.md` declares the engine as **Unity**, but the repository was
bootstrapped as an **Unreal Engine 5.3** project (PR #3, closing issue #2,
which had explicitly asked for Unreal). Issues #5, #7, and #8 then pivoted
the project to Unity. Issue #8 asks specifically to "study the documentation
and adapt to the Unity engine" — i.e. **align the codebase with the Unity
direction declared in the design doc**.

PR #9 implements that alignment:

1. Replaces the UE5 project skeleton (`OneTry.uproject`, `Config/`, `Content/`)
   with a minimal-but-valid Unity 6.3 LTS project skeleton (`ProjectSettings/`,
   `Packages/manifest.json`, `Assets/`).
2. Replaces the UE5 packaging workflow with a GameCI-based Unity build
   workflow that produces a portable Windows EXE ZIP artifact.
3. Updates `README.md` to reflect the Unity workflow.
4. Documents the required Unity packages for the design-doc mechanics
   (Input System, Cinemachine, URP, etc.) in `solutions.md` so subsequent
   PRs (#5, #7) can build on a consistent foundation.
