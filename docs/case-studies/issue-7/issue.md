# Issue #7 — подготовь абстрактные классы UNITY для гибкости

**URL:** https://github.com/Jhon-Crow/One-try/issues/7
**Author:** @Jhon-Crow
**State:** OPEN
**Filed:** 2026-04-25T20:04:02Z

## Original text (Russian)

**Title:** подготовь абстрактные классы UNITY для гибкости

**Body:**

> создай унифицированные абстрактные классы, чтоб и игрок и враг и любые
> существа наследовались от одного класса.
> чтоб на всех существ можно было накладывать любые статусы или эффекты.
> чтоб все существа могли использовать любое оружие.
> чтоб оружие можно было легко модифицировать, можно было дать любому оружию
> любые снаряды и эффекты (даже использовать в качестве снаряда существо или
> оружие или что угодно).
> в общем чтоб в потенциале можно было всем влиять на вся (например враг враг
> использовал способность на игрока - отнял у него оружие, зарядил оружие
> игрока в своё и выстрелил другим врагом (которые являются дефолтными
> зарядами оружия врага) который, вооружён оружием игрока, но игрок прожал
> эффект предмета и сделал пули лечащими или врага дружественным -  вот такого
> плана взаимодействия должна позволять инфраструктура).
> если возможно подготовь абстрактный класс для AI (GOAP), чтоб можно было
> добавить искусственный интеллект и любое поведение чему угодно.
>
> Please collect data related about the issue to this repository, make sure
> we compile that data to `./docs/case-studies/issue-{id}` folder, and use it
> to do deep case study analysis (also make sure to search online for
> additional facts and data), and propose possible solutions (including known
> existing components/libraries, that solve similar problem or can help in
> solutions).
> и реализуй

## Translation / summary (English)

> Create unified abstract classes so that the player, enemies, and any
> creatures all inherit from one base class.
>
> Any status or effect must be applicable to any creature.
>
> Every creature must be able to use any weapon.
>
> Weapons must be easy to modify; any weapon must be able to be loaded with
> any projectile and any effect (even using a creature, a weapon, or anything
> else as a projectile).
>
> In general, it should be possible for everything to influence everything.
> Example: an enemy uses an ability on the player — takes the player's weapon
> away, loads the player's weapon into the enemy's weapon, and fires another
> enemy (those enemies are the default ammo of the enemy's weapon) who is
> wielding the player's weapon — but the player triggers an item effect and
> makes the bullets healing, or the enemy friendly — this kind of interaction
> should be allowed by the infrastructure.
>
> If possible, prepare an abstract class for AI (GOAP) so artificial
> intelligence and any behavior can be added to anything.
>
> Compile data into `docs/case-studies/issue-{id}/`, do a deep analysis,
> propose solutions (including existing libraries/components), and implement
> the result.

## Explicit requirements (extracted)

| #  | Requirement                                                                                                                       |
|----|-----------------------------------------------------------------------------------------------------------------------------------|
| R1 | Single unified abstract base class (or interface) that Player, Enemy, and any "creature" inherit from.                            |
| R2 | Any status / effect can be applied to any creature.                                                                               |
| R3 | Any creature can wield / use any weapon.                                                                                          |
| R4 | Weapons are easily modifiable. Any weapon can accept any projectile and any effect.                                               |
| R5 | A projectile can be anything — a creature, a weapon, an item, an arbitrary game object.                                           |
| R6 | The system supports cross-cutting interactions: stealing weapons, loading weapons inside weapons, swapping behaviors at runtime.  |
| R7 | Effects can mutate behavior at the projectile level (e.g. bullets become healing, enemies become friendly).                       |
| R8 | If possible, an abstract class / interface for AI (GOAP — Goal-Oriented Action Planning) so behavior can be attached to anything. |
| R9 | Implement the design — not just describe it.                                                                                      |
| R10| Compile case study to `docs/case-studies/issue-7/`, search online, and reference existing components/libraries.                    |

## Out of scope (this iteration)

- Concrete enemy AI behaviors (only the GOAP scaffolding).
- Final weapon/projectile content — only enough to demonstrate the system.
- Animation / VFX / SFX integration.
- Networking / multiplayer.
- Save/load (the project explicitly has "no saves" — see `GAME_DESIGN.md`).
- Pickup/inventory UI (a runtime API is enough; UI can come later).
