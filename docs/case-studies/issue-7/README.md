# Case study — Issue #7: подготовь абстрактные классы UNITY для гибкости

This folder collects all design data, references, and analysis for issue #7,
which requested a flexible, composition-first abstraction layer for
creatures, statuses, weapons, projectiles, and AI.

| File | Purpose |
|---|---|
| [issue.md](./issue.md) | Original issue text (Russian) + English translation + extracted requirements R1–R10. |
| [analysis.md](./analysis.md) | Deep technical analysis: architectural patterns, engine constraints, library survey, design rationale. |
| [proposed-solutions.md](./proposed-solutions.md) | Comparison of options and selected solution with file-by-file breakdown. |
| [references.md](./references.md) | All consulted sources (Unity docs, GOAP papers, GAS, surveyed libraries). |
| [raw/](./raw/) | Raw data: issue JSON snapshot. |

## TL;DR

The issue asks for one base class for player + enemies, status effects that
work on anything, weapons that any creature can wield, projectiles that can
be anything (even other creatures), and a GOAP scaffold for AI.

### Selected approach

A self-contained, composition-first framework in `Assets/Scripts/Core/`:

- **`Creature : MonoBehaviour`** — the single aggregate base. Holds
  references to small, focused components. No game logic of its own.
- **`HealthComponent`, `Resource`, `StatusEffectController`,
  `WeaponHolder`, `AgentBrain`** — the swappable behaviour modules.
- **`IStatusEffect`, `IWeapon`, `IProjectile`, `IProjectileFactory`,
  `IOnHitAction`, `IAction`, `IGoal`, `IPlanner`** — the eight interfaces
  through which everything talks.
- **`Planner`** — a small A* GOAP planner over `(WorldState, Action)`
  that satisfies R8 without external packages.

All cross-cutting interactions described in the issue (steal weapon, load
weapon-into-weapon, fire creature-as-projectile, healing bullets, friendly
faction switch) are implemented as plain data transfers between components
and verified by EditMode tests.

### Why composition over inheritance

A deep inheritance tree (`Player : Humanoid : LivingThing : Pawn`) breaks
the moment requirement R6 ("everything affects everything") asks for
runtime swaps of identity. Composition keeps every relation as a mutable
reference, so swaps are free and tests are tight.

See [analysis.md §3](./analysis.md#3-why-a-flat-composition-tree-beats-deep-inheritance)
for the full rationale and [proposed-solutions.md §"Option A — design in
detail"](./proposed-solutions.md#option-a--design-in-detail) for the
file-by-file breakdown.

### Files shipped in this PR

```
Assets/Scripts/Core/
├── OneTry.Core.asmdef
├── Entities/         (5 files)
├── Statuses/         (effects + controller + modifiers)
├── Combat/           (weapons, projectiles, on-hit actions)
└── Ai/               (GOAP — planner, world state, goals, actions)
Assets/Tests/EditMode/
├── OneTry.Tests.EditMode.asmdef
└── (5 test files covering R1–R8)
```

### Out of scope (explicitly)

- Concrete enemy AI behaviours (only the GOAP scaffolding is shipped).
- Animation, VFX, SFX integration.
- Inventory UI.
- Networking.
- A new mini-boss or final-boss roster.

The framework exists so those features can be added without rewriting
the core when they are scheduled.
