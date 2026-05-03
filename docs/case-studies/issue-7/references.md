# References — Issue #7

All sources consulted while designing the abstraction framework.

## Repository data (Jhon-Crow/One-try)

| Item | URL |
|---|---|
| Issue #7 | https://github.com/Jhon-Crow/One-try/issues/7 |
| PR #14 (this PR) | https://github.com/Jhon-Crow/One-try/pull/14 |
| Game Design Document | `GAME_DESIGN.md` (this repo) |
| Existing player scripts | `Assets/Characters/Player/Scripts/PlayerCharacter.cs` |
| Issue #5 case study (player model) | `docs/case-studies/issue-5/` |
| Issue #8 case study (Unity migration) | `docs/case-studies/issue-8/` |
| Issue #10 case study (CI / portable EXE) | `docs/case-studies/issue-10/` |

## Unity engine — official docs

| Title | URL |
|---|---|
| Unity 6.3 LTS — release | https://unity.com/blog/unity-6-3-lts-is-now-available |
| Unity Manual — `MonoBehaviour` | https://docs.unity3d.com/ScriptReference/MonoBehaviour.html |
| Unity Manual — Component-Based Architecture (intro) | https://docs.unity3d.com/Manual/UsingComponents.html |
| Unity Manual — `ScriptableObject` | https://docs.unity3d.com/ScriptReference/ScriptableObject.html |
| Unity Manual — Assembly Definitions | https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html |
| Unity Test Framework — getting started | https://docs.unity3d.com/Packages/com.unity.test-framework@1.4/manual/index.html |
| Unity Input System | https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/index.html |
| Unity Behaviour package (preview) | https://docs.unity3d.com/Packages/com.unity.behavior@latest |
| Unity DOTS / ECS overview | https://unity.com/dots |

## GOAP — academic and engineering references

| Title | URL |
|---|---|
| Jeff Orkin — "Three States and a Plan: The A.I. of F.E.A.R." (GDC 2006) | https://alumni.media.mit.edu/~jorkin/gdc2006_orkin_jeff_fear.pdf |
| Jeff Orkin — Goal-Oriented Action Planning (paper) | https://alumni.media.mit.edu/~jorkin/goap.html |
| GameDev.net — "GOAP Tutorial Part 1" | https://gamedev.net/tutorials/programming/artificial-intelligence/goal-oriented-action-planning-r4117/ |
| AI and Games — "F.E.A.R. AI" (video, summarising Orkin's work) | https://www.youtube.com/watch?v=9oHX8QOWG2I |
| Wikipedia — A* search algorithm | https://en.wikipedia.org/wiki/A*_search_algorithm |

## GOAP / AI implementations surveyed

| Library / project | URL | Used as |
|---|---|---|
| `crashkonijn/GOAP` | https://github.com/crashkonijn/GOAP | Reference for production-grade Unity GOAP design (sensors, world keys, runner, debugger). |
| `abide-by-recursion/goap-cs` | https://github.com/abide-by-recursion/goap-cs | Reference for a minimal pure-C# A* GOAP planner. |
| `sploreg/goap` (Brent Owens, classic) | https://github.com/sploreg/goap | Historical Unity GOAP reference. |
| Unity Behaviour package | https://docs.unity3d.com/Packages/com.unity.behavior@latest | Compared with GOAP — kept GOAP per issue request. |

## Combat / status / weapon framework references

| Source | URL | Why |
|---|---|---|
| Unreal Engine — Gameplay Ability System (GAS) overview | https://docs.unrealengine.com/5.4/en-US/gameplay-ability-system-for-unreal-engine/ | Inspiration for `StatusEffect` lifecycle (apply/tick/remove) and modifier chaining. |
| Unreal — `UGameplayEffect` reference | https://docs.unrealengine.com/5.4/en-US/API/Plugins/GameplayAbilities/UGameplayEffect/ | Same. |
| Game Programming Patterns — Component | https://gameprogrammingpatterns.com/component.html | The architectural backbone we use. |
| Game Programming Patterns — Strategy | https://gameprogrammingpatterns.com/state.html | Modifier / on-hit pattern. |
| Game Programming Patterns — Observer | https://gameprogrammingpatterns.com/observer.html | Health / death event broadcasting. |

## Composition vs inheritance — background

| Source | URL |
|---|---|
| Eric Gamma et al., "Design Patterns" — "Favor object composition over class inheritance" | https://en.wikipedia.org/wiki/Composition_over_inheritance |
| Robert Nystrom — "Game Programming Patterns" book site | https://gameprogrammingpatterns.com/ |
| Unity blog — "Entity Component System" | https://unity.com/blog/engine-platform/entity-component-system-for-unity-getting-started |

## Libraries we explicitly chose **not** to bundle (and why)

| Library | Why declined |
|---|---|
| Opsive — Ultimate Character Controller | Paid, monolithic, conflicts with the "easily modifiable" requirement. |
| Animancer | Paid; animation layer is not in scope for this issue. |
| `com.unity.behavior` (Behaviour package) | Behaviour Trees are not GOAP; the issue explicitly asks for GOAP. We can layer BTs on top later. |
| Unity DOTS / ECS | Authoring overhead too high for a prototype; revisit when entity counts cross ~1000. |
| External event-bus packages (`MessagePipe`, `Zenject`/`Extenject`) | Adds DI overhead and learning curve; standard `event Action` is sufficient at this scale. |

## Related case studies (this repository)

| Issue | Location | Summary |
|---|---|---|
| Issue #5 | `docs/case-studies/issue-5/` | Player model (mannequin prefab + scripts) — predecessor of the new `Creature` base. |
| Issue #8 | `docs/case-studies/issue-8/` | UE5 → Unity migration — explains why we are in Unity / why the abstractions are in C#. |
| Issue #10 | `docs/case-studies/issue-10/` | CI portable-EXE secret gap — out of scope here, but relevant for verifying the build still works. |
