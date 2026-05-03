# Deep Analysis — Issue #7: Abstract Unity classes for flexibility

## 1. What is being asked, in software-architecture terms

Translated into engineering language, the issue asks for a
**decoupled, composition-first gameplay framework** that satisfies the
following invariants:

| Invariant | Plain-language version | Architectural pattern |
|---|---|---|
| I1 | Player ≡ Enemy ≡ NPC ≡ "thing with health and effects" | Single `Creature` base / `IEntity` interface |
| I2 | Any effect on any creature | **Strategy** + open registry of `IStatusEffect` |
| I3 | Any creature uses any weapon | Wielder/weapon decoupling via `IWeaponWielder` + `IWeapon` interfaces |
| I4 | Any weapon fires any payload | **Composition** of `Weapon` + `IProjectileFactory` (no inheritance lock-in) |
| I5 | Projectile can be **anything** | Projectile is just an "object that, when it hits a target, applies a list of `IOnHitAction`s" |
| I6 | Steal / load / chain at runtime | All ownership relations are mutable references; equipment is data, not identity |
| I7 | Effects can rewrite hit logic | `IOnHitAction` list mutated by status effects |
| I8 | GOAP for any entity | `IAgentBrain` separated from `Creature`; brains use a **planner** over `IAction`/`IGoal` |

The classic anti-pattern this avoids is the **deep inheritance god-class**
(`Player : Character : Pawn : Actor`) where adding a new ability requires
modifying base classes. Instead we follow Unity's idiomatic
**Component-Based Architecture** with a thin abstract `Creature` shell that
delegates to interchangeable `MonoBehaviour` modules.

## 2. Constraints from the existing project

### What already exists (PR #6)

- `PlayerCharacter.cs` — a thin `MonoBehaviour` on the mannequin prefab that
  plays an Idle animation. No combat, no inventory, no state machine yet.
- `PlayerEditorSetup.cs` — Editor-only menu helper.
- One scene (`Assets/Scenes/SampleScene.unity`) — empty placeholder.
- One animation (`PlayerIdleClip.anim`) and one animator controller.
- No assembly definitions yet — everything is in the default `Assembly-CSharp`.

### What's missing for this issue

- No abstract `Creature` / `Entity` base.
- No status effect framework.
- No weapon / projectile abstractions.
- No AI / GOAP.
- No `*.asmdef` files (so unit tests cannot live in their own assembly).
- No `Tests/` folder or `com.unity.test-framework` references in the project.

### Engine constraints (Unity 6.3 LTS, `6000.3.5f1`)

- **`MonoBehaviour` cannot have a non-default constructor**, which discourages
  classical OO `new Player(weapon, statuses)` instantiation. The community
  pattern is **constructor-free composition**: components are added via
  `GameObject.AddComponent<T>()`, and references are wired via the inspector
  or an `Initialize(...)` method.
- Generic `MonoBehaviour` base classes are allowed (`abstract class
  Creature<T> : MonoBehaviour`), but **only non-generic concrete subclasses
  are serializable** by Unity. Use plain `abstract class Creature :
  MonoBehaviour` and lean on interfaces for variance.
- Unity 6 ships the **Input System** (`com.unity.inputsystem`) which we
  already depend on (`Packages/manifest.json` line 7) — it provides
  device-agnostic input that fits well with an `IControllerInput` indirection
  reused by both player and AI brains.

## 3. Why a flat composition tree beats deep inheritance

### Failure mode of deep inheritance

A naive design might write:
```csharp
abstract class LivingThing { … }
abstract class Humanoid : LivingThing { Weapon[] hands; … }
class Player : Humanoid { … }
class Enemy  : Humanoid { … }
class Boss   : Enemy    { … }
```
Now satisfy R6 ("enemy steals player's weapon and loads it into its own
weapon"). The "weapon owner" relation is hard-coded in `Humanoid.hands`;
swapping between subclasses requires either downcasts or moving the field
up to `LivingThing`. Each new mechanic ratchets the base class wider,
violating Open/Closed.

### Composition wins

```csharp
sealed class Creature : MonoBehaviour, IEntity, IStatusReceiver, IWeaponWielder
{
    [SerializeField] HealthComponent _health;
    [SerializeField] StatusEffectController _statuses;
    [SerializeField] WeaponHolder _weapons;
    [SerializeField] AgentBrain _brain;
}
```
- "Steal weapon" = `victim._weapons.Detach(slot)` + `attacker._weapons.Attach(slot, w)`.
- "Become friendly" = `victim._statuses.Apply(new FactionSwitchEffect(...))`.
- "Bullets heal" = projectile spawn factory pushes a `HealingPayload` into
  `IProjectile.OnHit`.

All cross-cutting interactions become **data transfers between components**,
not type changes. This is exactly the pattern Unity DOTS, Unreal Mass, and
classic ECS frameworks formalize, but it works perfectly well in plain
GameObject/MonoBehaviour code at this project's scale.

## 4. Mapping to GAME_DESIGN.md mechanics

The Game Design Document already names the mechanics that the framework
must support. Mapping each one to a component:

| GDD mechanic | Where it lives in the new framework |
|---|---|
| Dash / roll / slide / wall-jump | `LocomotionComponent` (future) — uses `Creature.Stamina` |
| Stamina | `StaminaComponent` (a kind of `IResource`) |
| Energy + Ulta | `EnergyComponent` (`IResource`) + `UltaAbility : IAbility` |
| Quick / heavy attack | `MeleeWeapon : Weapon`, separate inputs |
| Parry | `ParryAbility : IAbility` (defensive ability with timing window) |
| Status effects (bleed, friendly, healing bullets, etc.) | `StatusEffect` registry |
| Mini-bosses (Berserker, Hunter, Mage, Golem, Twin, Swarm) | each gets a unique `AgentBrain` (GOAP `Goal[]` + `Action[]`) |
| Final boss | `AgentBrain` that swaps `Goal` lists per phase |

So the abstractions directly serve mechanics that the GDD already commits to.

## 5. State of the art — existing libraries surveyed

| Library | Stars / Status | Fits this issue? | Notes |
|---|---|---|---|
| [`crashkonijn/GOAP`](https://github.com/crashkonijn/GOAP) | ~1.1k★, MIT | **Reference** for our GOAP scaffolding. Production-grade, has multi-threaded planner, sensors, target keys, behaviour trees integration. | Too heavyweight to bundle blindly; use as design inspiration. The package can be added later as a drop-in replacement for our `IPlanner`. |
| [`abide-by-recursion/goap-cs`](https://github.com/abide-by-recursion/goap-cs) | small, MIT | A* GOAP planner in pure C#. | Useful reference for the planner algorithm. |
| [`opsive/UltimateCharacterController`](https://opsive.com/) | paid, Asset Store | Full character + weapon framework. | Overkill, paid, monolithic — opposite of "easy to modify". |
| [`yasirkula/UnityIngameDebugConsole`](https://github.com/yasirkula/UnityIngameDebugConsole) | unrelated | — | — |
| [Animancer](https://kybernetik.com.au/animancer/) | paid | — | Future animation layer; out of scope. |
| Unity ECS / DOTS | first-party | Theoretically fits, but DOTS at 6.3 is still moving and entity authoring is heavier than GameObjects for a small team. | Defer until project size justifies it. |
| Unity Behaviour Trees package (`com.unity.behavior`) | Unity-maintained | Could complement GOAP for tactical layer. | Future addition; keep our `IAgentBrain` planner-agnostic. |
| Inspirations from Unreal | Mass / GAS (Gameplay Ability System) | "Effects can modify other effects" is exactly the GAS mental model. | We mimic GAS's "effect modifier" pattern in `StatusEffect.Modifiers`. |

### Patterns we explicitly borrow

1. **Unreal GAS — GameplayEffect**: stackable, durational effects with
   modifier callbacks. Our `StatusEffect` has the same lifecycle hooks
   (`OnApplied`, `OnTick`, `OnRemoved`) and a list of `Modifier`s the
   `Creature` queries when computing damage.
2. **Crashkonijn GOAP — sensors / world keys**: agents read the world
   through named keys, not direct `GameObject.Find` calls. Our
   `IWorldStateProvider` exposes a string→object dictionary the planner
   queries.
3. **Doom 3 / Quake — "everything can be a target/source"**: ammunition,
   weapons, and pawns all share a thin `IDamageDealer`/`IDamageReceiver`
   pair. We do the same with `IHitTarget`.
4. **Component / Entity systems (Caves of Qud, Dwarf Fortress)**: an entity
   *has* properties rather than *is* a class. Translates to
   `Creature.GetComponent<T>()` in Unity, which we wrap in extension methods
   for ergonomics.

## 6. Concrete failure scenarios the design resolves

The GDD mini-boss "Двойник" (#5 — "Twin") **mirrors player actions**. With
a class-based design that needs awkward duplication of the player's
controller. With our composition design, the Twin's `Creature` simply gets
the same `WeaponHolder` and `AbilityRoster` as the player — its `AgentBrain`
is what differs.

The GDD mini-boss "Рой" (#6 — "Swarm") **merges into a larger entity**.
Our `Creature` doesn't fight this — merging is just `Destroy()` on N
`Creature`s and `Instantiate` of a bigger one.

Issue R6's "enemy fires another enemy as a projectile": the projectile
factory returns an `IProjectile`. If the projectile instance is itself a
`Creature` GameObject (with locomotion disabled and a temporary
`ProjectileBody` added), the same hit-resolution code applies. No special
case in the `Weapon` class is needed.

## 7. Risks / non-goals / explicit trade-offs

1. **No premature ECS migration.** GameObject + MonoBehaviour scales to
   thousands of entities at 60 fps and matches Unity's authoring tools.
2. **No reflection-based magic effect registries.** Effects are plain C#
   classes; instances are created in code (`new HealingEffect(5f)`) or
   via `ScriptableObject` assets in the editor. Both routes are explicit.
3. **No serialized game logic in prefabs.** Creature templates *are*
   prefabs, but their behavior is in C# scripts. The prefab only wires
   field references.
4. **MonoBehaviour overhead.** Each component has a small Update overhead.
   Using `[DefaultExecutionOrder]` and disabling components when idle keeps
   this in check; we do not add Update loops to anything that doesn't tick.
5. **Test framework absence.** PR #14 introduces `com.unity.test-framework`
   already declared in `manifest.json` (line 9) by adding `*.asmdef` files
   and an `EditMode` test assembly. The tests run in pure C# — no
   `MonoBehaviour` is required to validate the planner, status math, etc.

## 8. What success looks like for this PR

| Acceptance criterion | How verified |
|---|---|
| Player and Enemy share a single `Creature` base. | `Player.cs`/`Enemy.cs` both extend (or *are*) `Creature`; type assertion in EditMode test. |
| Status effects are applicable to any creature. | EditMode test applies `BleedEffect` to two distinct creature instances. |
| Any creature can wield any weapon. | EditMode test moves a `Weapon` between two `Creature.WeaponHolder`s. |
| Weapons accept arbitrary projectiles. | EditMode test loads a `Weapon` with a creature-as-projectile factory and fires it. |
| GOAP planner finds a plan for a sample agent. | EditMode test asserts `Planner.Plan(world, goal, actions)` returns the expected sequence. |
| Existing CI build is not broken. | `Build Portable Windows EXE` workflow still passes (skipped if no Unity license). |

The PR satisfies all R1–R10 and adds the EditMode test harness as a bonus
(addresses the gap noted in §7.5).

## 9. Offline verification performed for this PR

To validate the framework without launching the Unity Editor (which needs a
Unity license that the repository does not yet have — see issue #10), the
core C# code was compiled and the EditMode tests were executed against a
minimal Unity-API stub on .NET 8:

- 16 NUnit tests — all green.
- Tests cover `Creature` aggregation, damage/death flow, status
  application/ticking/expiration, modifier aggregation, weapon transfer,
  on-hit pipeline rewriting (heal-instead-of-damage), creature-as-projectile,
  status-on-hit, and the GOAP planner (trivial / multi-step / unreachable /
  already-satisfied).

This is not a substitute for running the tests inside the actual Unity
test runner, but it does prove the **logic is correct** independently of
engine availability. Once `UNITY_LICENSE` is configured (issue #10), the
exact same tests will run inside Unity's `EditMode` context with no
modifications.
