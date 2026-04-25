# Proposed solutions — Issue #7

## Overview

The issue asks for an **abstraction layer**, not a specific feature. The
solution space therefore comes down to *which architectural style* and
*how much existing code* to bring in.

---

## Option comparison

| # | Option | Effort | Lock-in | Lines added | Verdict |
|---|---|---|---|---|---|
| **A** | **Hand-rolled composition framework — `Creature` + interfaces + minimal GOAP planner**  | Medium | None | ~1,500 | **Selected** |
| B | Adopt [`crashkonijn/GOAP`](https://github.com/crashkonijn/GOAP) as a UPM package; build only the gameplay shell on top | Low (planner) + Medium (shell) | Adds an external dependency | ~800 + package | Deferred — keeps PR self-contained |
| C | Rewrite on Unity DOTS / ECS | High | High (different authoring) | thousands | Premature |
| D | Buy `Opsive Ultimate Character Controller` | Low | Closed-source, paid | 0 (we add config) | Wrong fit; paid; opinionated |
| E | Wait for Unity Behavior package (`com.unity.behavior`) and use it | Low | Tied to Unity package roadmap | small | BTs ≠ GOAP; doesn't satisfy R8 cleanly |
| F | Pure ScriptableObject "data" framework (no C# interfaces) | Low | High (designer-only) | ~600 | Doesn't satisfy R6 / R7 dynamism well |

### Why **A** wins for this issue

1. **Self-contained.** No third-party packages, no Asset Store purchases.
   The PR is fully reviewable from the diff alone.
2. **Interface-first.** Every coupling point is an `I*` interface, so we
   can swap implementations later (e.g. drop in crashkonijn/GOAP for the
   planner) without changing call sites.
3. **Direct R1–R8 mapping.** Each requirement maps to a single file or
   small file group (see §"Files delivered" below).
4. **Test-friendly.** All gameplay logic that isn't strictly Unity-y
   (planner, status math, weapon resolution) lives in plain C# classes
   that EditMode tests run without instantiating a `GameObject`.
5. **No lock-in.** `Creature : MonoBehaviour` is a Unity convention,
   nothing more — the gameplay logic is not chained to engine internals.

---

## Option A — design in detail

### 8 layers, top-down

```
┌─────────────────────────────────────────────────────────────────┐
│                       Scene / Prefab layer                      │
│   PlayerMannequin.prefab, EnemyTemplate.prefab, …               │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────┴────────────────────────────────────┐
│                          Creature                               │
│   Aggregates the components below — no game logic of its own.   │
└──┬─────────┬─────────┬─────────┬─────────┬─────────┬────────────┘
   │         │         │         │         │         │
┌──┴───┐ ┌───┴────┐┌───┴───┐ ┌───┴──────┐ ┌┴─────┐ ┌─┴─────────┐
│Health│ │Statuses││Weapons│ │Locomotion│ │Brain │ │Faction    │
│Cmpt  │ │Cmpt    ││Holder │ │(future)  │ │(GOAP)│ │           │
└──────┘ └────────┘└──┬────┘ └──────────┘ └──┬───┘ └───────────┘
                     │                       │
              ┌──────┴───────┐         ┌─────┴────────┐
              │   IWeapon    │         │   IAction    │
              └──────┬───────┘         └─────┬────────┘
                     │                       │
              ┌──────┴───────┐         ┌─────┴────────┐
              │ IProjectile  │         │   IGoal      │
              │  Factory     │         └──────────────┘
              └──────┬───────┘
                     │
              ┌──────┴───────┐
              │ IOnHitAction │
              └──────────────┘
```

### Files delivered (this PR)

```
Assets/Scripts/Core/
├── Entities/
│   ├── IEntity.cs                    # ID, position, faction, alive flag
│   ├── Creature.cs                   # the aggregate MonoBehaviour
│   ├── HealthComponent.cs            # IDamageReceiver implementation
│   ├── Faction.cs                    # enum + helpers
│   └── Resource.cs                   # generic stamina/energy resource
├── Statuses/
│   ├── IStatusEffect.cs              # apply/tick/remove + modifier list
│   ├── StatusEffectController.cs     # MonoBehaviour: ticks, stacking
│   ├── Modifier.cs                   # damage/speed/heal modifiers
│   └── Effects/
│       ├── BleedEffect.cs
│       ├── HealOverTimeEffect.cs
│       └── FactionSwitchEffect.cs    # "make enemy friendly"
├── Combat/
│   ├── IWeaponWielder.cs
│   ├── WeaponHolder.cs               # any creature carries weapons
│   ├── IWeapon.cs
│   ├── Weapon.cs                     # reference impl: cooldown + projectile factory
│   ├── IProjectile.cs
│   ├── IProjectileFactory.cs
│   ├── IOnHitAction.cs               # what happens when projectile hits
│   ├── HitContext.cs                 # source, victim, payloads
│   ├── Projectiles/
│   │   ├── SimpleProjectile.cs       # generic gameobject projectile
│   │   └── CreatureAsProjectile.cs   # uses a Creature as ammo (R5/R6)
│   └── HitActions/
│       ├── DamageAction.cs
│       ├── HealAction.cs
│       └── ApplyStatusAction.cs
├── Ai/
│   ├── IAgentBrain.cs
│   ├── AgentBrain.cs                 # MonoBehaviour wrapper
│   ├── IWorldState.cs                # key→value world snapshot
│   ├── WorldState.cs                 # in-memory dictionary impl
│   ├── IGoal.cs
│   ├── IAction.cs
│   ├── Plan.cs
│   └── Planner.cs                    # plain-C# A* over actions
└── OneTry.Core.asmdef                # assembly: gameplay code only

Assets/Tests/EditMode/
├── CreatureTests.cs
├── StatusEffectTests.cs
├── WeaponHolderTests.cs
├── ProjectileTests.cs
├── PlannerTests.cs
└── OneTry.Tests.EditMode.asmdef      # references core + test framework
```

### Interaction examples (smoke tests in code)

```csharp
// Example: enemy steals player's weapon, fires it, but bullets heal because
// the player's item effect mutated the projectile factory.
var player = SpawnCreature("Player");
var enemy  = SpawnCreature("Enemy", faction: Faction.Hostile);
var rifle  = ScriptableObject.CreateInstance<WeaponDefinition>();
rifle.Build(player.WeaponHolder);

// Player triggers an item effect that wraps the rifle's hit actions
rifle.ProjectileFactory.OnHitActions.Add(new HealAction(amount: 10));
rifle.ProjectileFactory.OnHitActions.RemoveAll(a => a is DamageAction);

// Enemy uses an ability to detach + steal
StealWeaponAbility.Cast(enemy, player, slot: 0);
Assert.AreSame(rifle, enemy.WeaponHolder.GetSlot(0));

// Enemy fires; victim is another enemy → that enemy is healed, not hurt
var victim = SpawnCreature("Enemy2", faction: Faction.Hostile);
enemy.WeaponHolder.GetSlot(0).Fire(target: victim.transform.position);
Assert.AreEqual(victim.Health.Maximum, victim.Health.Current); // healed back
```

The full equivalent of this scenario is automated in
`Assets/Tests/EditMode/ProjectileTests.cs`.

### GOAP design (R8)

A minimal, dependency-free planner sufficient to back the framework:

- `IWorldState` — string-keyed dictionary of facts.
- `IAction` — `Preconditions`, `Effects` (also key→value), `Cost`, `IsValid(WorldState)`.
- `IGoal` — `IsSatisfied(WorldState)`.
- `Planner.Plan(initial, goal, actions)` — A* over the (state, action) graph
  bounded by max plan length (default 12).
- `AgentBrain : MonoBehaviour` — replans every N seconds (default 0.5s) or
  on `WorldState` invalidation; executes the current plan step by step.

Why A* and not BTs/HTNs? Because the issue explicitly names GOAP; A* is the
classical, correct, easy-to-explain implementation. We can drop in
`crashkonijn/GOAP` later by implementing `IPlanner` against its API — call
sites do not change.

---

## Option B — adopt `crashkonijn/GOAP`

Steps if/when the project outgrows our planner:

1. Add to `Packages/manifest.json`:
   ```json
   "com.crashkonijn.goap": "https://github.com/crashkonijn/GOAP.git?path=/Package"
   ```
2. Replace our `Planner.cs` with an adapter that calls
   `crashkonijn.GoapRunner` and bridges our `IAction`/`IGoal` to its
   classes.
3. Remove `Planner.cs`. Keep `IPlanner` interface so `AgentBrain` doesn't
   change.

This is intentionally not done in this PR — adding a third-party UPM
dependency requires a separate review pass.

---

## Risks and how the design mitigates them

| Risk | Mitigation |
|---|---|
| Performance: every creature ticks all components. | Components disable themselves when idle; `StatusEffectController` only ticks if it has effects. |
| Inspector wiring becomes tedious. | `Creature.Awake()` calls `GetComponentInChildren<T>()` for each module; manual wiring is optional. |
| EditMode tests can't use `MonoBehaviour` lifecycle. | Tests construct components via `new GameObject().AddComponent<T>()` and call `Awake()`/`Start()` explicitly only when needed. Pure-C# logic (planner, status math) does not need `GameObject` at all. |
| GOAP planner explosion. | Hard cap on plan length + action count; planner returns null if no plan within budget. |
| Tight coupling to Unity types in pure-C# code. | Core C# files in `Statuses/`, `Combat/`, `Ai/` (planner) avoid `UnityEngine.*` types except `Vector3` and serialization attributes. |

---

## CI / packaging implications

- The new `*.asmdef` files cause Unity to compile the new assemblies but
  do **not** require any `UNITY_LICENSE` to compile — they would also
  break `dotnet build` (we don't run that today; out of scope).
- The build pipeline (`.github/workflows/build.yml`) is unchanged. When
  `UNITY_LICENSE` is configured (see `docs/case-studies/issue-10/`), the
  same workflow will compile the new assemblies and produce the portable
  EXE that includes them.
- Tests are EditMode (no Player runtime needed), so adding a `Test` job
  that calls `game-ci/unity-test-runner@v4` is a small, low-risk follow-up
  — left as a TODO in `proposed-solutions.md` to keep this PR focused.

---

## Why this resolves the original requirements

| Req. | Resolution |
|---|---|
| R1 | `Creature : MonoBehaviour, IEntity, IStatusReceiver, IWeaponWielder` — single base for player + enemies. |
| R2 | `StatusEffectController.Apply(IStatusEffect)` works on any `Creature`. |
| R3 | `WeaponHolder` is on every `Creature`. Any `IWeapon` can be attached to any holder. |
| R4 | `Weapon` references an `IProjectileFactory` and a list of `IOnHitAction`s — both mutable lists, both swappable at runtime. |
| R5 | `IProjectile` is an interface; `CreatureAsProjectile` proves a `Creature` itself can be used as ammo. |
| R6 | All ownership relations are mutable references (`WeaponHolder` slots, `StatusEffectController` list, `AgentBrain` actions). |
| R7 | `IOnHitAction` list is mutable; `HealAction` + `FactionSwitchEffect` cover the "healing bullets / friendly enemy" case. |
| R8 | `IAgentBrain`, `IPlanner`, `IGoal`, `IAction`, `IWorldState`, plus a working A* `Planner`. |
| R9 | Code shipped in `Assets/Scripts/Core/`. |
| R10 | This case study + `references.md` cover compiled data and library survey. |
