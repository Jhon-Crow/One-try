# Deep Case-Study Analysis — Issue #8

## 1. Problem statement

Issue #8 (in Russian) reads:

> изучи документация и адаптируй под движок unity
> *Please collect data related about the issue to this repository, make sure
> we compile that data to `./docs/case-studies/issue-{id}` folder, and use it
> to do deep case study analysis (also make sure to search online for
> additional facts and data), and propose possible solutions (including
> known existing components/libraries, that solve similar problem or can
> help in solutions).*
> *и реализуй* — "and implement (it)"

Translated and condensed: **study the documentation, adapt the project to
the Unity engine, and implement the change**.

## 2. Repository state at the time of the issue

| Area | Current state | Source |
|---|---|---|
| Engine declared in design doc | **Unity** | `GAME_DESIGN.md` lines 5 & 200 ("Движок: Unity", "Технические приоритеты (Unity)") |
| Engine actually used in repo | **Unreal Engine 5.3** | `OneTry.uproject` (`"EngineAssociation": "5.3"`), `Config/Default*.ini`, `Content/Maps/` |
| CI build workflow | **UE5 BuildCookRun via Epic's Docker image** | `.github/workflows/build.yml` |
| README | Documents UE5 setup and EPIC_GITHUB_TOKEN flow | `README.md` |

There is also tooling debt in the existing UE5 setup: the build workflow
requires the repo owner's GitHub account to be linked to an Epic Games
account that accepted the EULA, plus a personal access token stored as
`EPIC_GITHUB_TOKEN`. This is a non-trivial barrier even before considering
the engine mismatch.

## 3. History — how the mismatch happened

Pulled from `raw/all-issues.json` and `raw/all-prs.json`:

| # | Type | Title (translated) | State |
|---|---|---|---|
| 1 | issue | "plan and structure" | closed |
| 2 | issue | "make an empty 3D scene on Unreal" | closed |
| 3 | PR | "Empty 3D scene on Unreal Engine 5 + portable EXE build in GitHub Actions" | merged |
| 4 | PR | "Structure and plan of One-try" | merged |
| 5 | issue | "add player model" | open |
| 7 | issue | "prepare abstract **UNITY** classes for flexibility" | open |
| 8 | issue | "study documentation and adapt to **Unity** engine" | open |

Sequence: the project was bootstrapped on **Unreal** (issue #2 → PR #3) at
the same time the design doc was authored mentioning **Unity** (PR #4).
Subsequent issues (#5 implicitly, #7 and #8 explicitly) pivot the project
to Unity. Issue #8 is therefore the *engine-pivot* ticket and is a
prerequisite to the others — there's no point implementing player model
work (#5) or "abstract Unity classes" (#7) on top of an Unreal skeleton.

## 4. Why Unity makes sense for this design

The `GAME_DESIGN.md` describes a Hyper Light Drifter / Hollow Knight /
Dark Souls-style 3D action roguelite with a heavy emphasis on **input
feel** (input buffering, coyote time, dash i-frames, parry windows). The
design explicitly references Unity in section "Технические приоритеты"
(Technical priorities).

Both engines can build this game, but for the team's current needs
Unity has clear advantages:

| Factor | Unity | Unreal Engine 5 |
|---|---|---|
| C# scripting | First-class, fast iteration | Blueprints + C++ (steeper) |
| Time-to-prototype | Minutes | Hours (compile cycles) |
| CI/headless build | Free CLI, mature OSS GameCI action | Requires Epic-linked GitHub account, large Docker image, EULA |
| Repository size | KBs (text assets) | Often MBs (binary `.uasset`) |
| Community examples for "feel" mechanics (dash, parry, coyote time, input buffer) | Abundant; nearly every tutorial targets Unity | Mostly Blueprint-based |
| 6.3 LTS released | Dec 2025, supported through Dec 2027 | UE 5.5 LTS expected later |

The single biggest issue with the current UE5 setup, beyond the design
mismatch, is operational: the CI build is gated on the repo owner's
personal Epic account. Migrating to Unity removes that coupling — the
GameCI flow needs only a Unity Personal license (free) and standard
GitHub secrets.

## 5. Constraints discovered during investigation

1. **Cannot run the Unity Editor in this environment.** That means we
   cannot generate the full set of `.meta` files or the binary-encoded
   `ProjectSettings/*.asset` files from scratch. We rely on Unity's
   built-in behavior of regenerating missing settings from `ProjectVersion.txt`
   on first open.
2. **Cannot run a real Unity build to verify the workflow end-to-end.**
   The GameCI workflow requires `UNITY_LICENSE` / `UNITY_EMAIL` /
   `UNITY_PASSWORD` repository secrets that are not present yet. The
   workflow is written so that the activation path is documented in
   `README.md` and the build job will execute as soon as the secrets
   are added.
3. **Existing UE5 workflow runs would fail anyway**, since they require
   the not-yet-configured `EPIC_GITHUB_TOKEN`. Removing them is a net
   improvement to CI signal.

## 6. Risks of the migration

- **Risk:** PR #6 (issue #5 — "add player model") is open and may have
  been authored against the UE5 skeleton.
  **Mitigation:** PR #6 is also still WIP / unmerged. After #9 lands the
  project will be on Unity, and #6 should be re-targeted there. Issue #7
  ("abstract UNITY classes") and #8 (this one) confirm the team's
  direction is Unity, so a re-target is consistent with stated intent.
- **Risk:** Loss of the UE5 design doc references that mention Unreal.
  **Mitigation:** The design doc already says Unity. The README is the
  only narrative file that mentions Unreal; we update it.
- **Risk:** Generated UE5 binary files / artifacts in someone's local
  checkout.
  **Mitigation:** UE5-specific entries in `.gitignore` are removed and
  replaced with Unity-specific ones. Local cleanup is the user's
  responsibility, but the repo no longer pushes them.

## 7. References

External documentation consulted while preparing this analysis:

- Unity 6.3 LTS announcement (released Dec 2025, supported Dec 2027) —
  <https://unity.com/blog/unity-6-3-lts-is-now-available>
- Unity 6 release & support matrix — <https://unity.com/releases/unity-6-releases>
- GameCI `unity-builder` GitHub Action — <https://github.com/game-ci/unity-builder>
- GameCI documentation — Builder reference: <https://game.ci/docs/github/builder/>
- GameCI documentation — Getting started: <https://game.ci/docs/github/getting-started/>
- GameCI example workflow: <https://github.com/game-ci/unity-actions-example>
- Unity Input System (official) — <https://github.com/Unity-Technologies/InputSystem>
- Unity package manifest (`manifest.json`) reference —
  <https://docs.unity3d.com/Manual/upm-manifestPrj.html>
- "Coyote time" + jump buffering discussion (Unity forums) —
  <https://discussions.unity.com/t/does-anyone-know-of-a-way-to-do-coyote-time-and-jump-buffering/786348>
