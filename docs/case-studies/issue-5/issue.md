# Issue #5 — Player Model

**Source:** https://github.com/Jhon-Crow/One-try/issues/5
**Author:** Jhon-Crow
**Opened:** 2026-04-25

## Original text (RU)

> добавь модельку игрока
>
> простого манекена (без оружия и других вещей)
> моделька должан быть обязательно многосоставной (плечи, предплечья, кисти и такая же детализация ног).
> учти что в будущем нужно будет стилизовать визуал под SIGNALIS
> так же сразу добавь idle анимацию для проверки.
> используй пресеты если необходимо.
>
> Please collect data related about the issue to this repository, make sure we compile that data to `./docs/case-studies/issue-{id}` folder, and use it to do deep case study analysis (also make sure to search online for additional facts and data), and propose possible solutions (including known existing components/libraries, that solve similar problem or can help in solutions).
> и реализуй

## Translation (EN)

> Add a player model.
>
> A simple mannequin (no weapons or other gear).
> The model **must** be multi-part (shoulders, forearms, hands, and the same level of detail for the legs).
> Note that in the future the visuals will need to be styled to match **SIGNALIS**.
> Also, immediately add an **idle animation** for verification.
> Use presets if needed.
>
> Compile related data into `./docs/case-studies/issue-5`, do a deep case-study analysis (also search online for facts and data), propose possible solutions (including known existing components/libraries that solve a similar problem or can help). And implement.

## Engine clarification (PR feedback)

The repo owner confirmed in a PR comment that the game uses **Unity**, not Unreal Engine
(per `GAME_DESIGN.md` — "Движок: Unity"). An earlier draft implemented this in UE5 and
was rejected. The implementation was rewritten for Unity 6.3 LTS.

## Hard requirements (extracted)

| # | Requirement | Acceptance criterion (Unity) |
|---|---|---|
| R1 | Player model exists in the project | A `PlayerMannequin.prefab` can be dropped into any scene and plays at runtime. |
| R2 | Mannequin only — no weapons or gear | The prefab has no weapon or prop GameObjects attached. |
| R3 | Multi-part hierarchy | Hierarchy exposes separate Transforms for clavicle, upper-arm, forearm, hand; thigh, calf, foot — independently animatable. |
| R4 | Future styling to SIGNALIS | Body segments are individually addressable; future swap replaces meshes or adds a skinned mesh without modifying scripts or animator. |
| R5 | Idle animation for verification | Animator Controller plays `PlayerIdleClip.anim` (breathing idle) on Start. |
| R6 | Use presets if needed | Unity built-in Capsule primitive used — a supported zero-dependency "preset". |

## Out of scope (this issue)

- SIGNALIS-styled materials / textures (planned as a follow-up).
- Locomotion blendspace, jump, attack animations.
- Networking / multiplayer.
