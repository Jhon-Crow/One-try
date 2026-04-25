# Case study — Issue #5: добавь модельку игрока

This folder collects the research and design rationale behind the player-model implementation.

| File | Purpose |
|---|---|
| [issue.md](./issue.md) | Original issue text (RU + EN translation), explicit requirements table, out-of-scope list. |
| [timeline.md](./timeline.md) | Reconstructed sequence of events from `git log` + GitHub comments, including the engine drift and the moment it was caught. |
| [root-cause.md](./root-cause.md) | Five-whys analysis for the wrong engine and the broken EXE pipeline, with the fixes and recurrence-prevention measures. |
| [analysis.md](./analysis.md) | Project context, Unity engine choice, multi-part primitive mannequin rationale, SIGNALIS aesthetic notes, build-environment constraints, validation plan, risks. |
| [proposed-solutions.md](./proposed-solutions.md) | Comparison of approaches and the selected solution (Unity primitive-based mannequin prefab). |
| [references.md](./references.md) | All consulted sources (SIGNALIS press kit, Unity docs, animation/humanoid references, GameCI). |

## TL;DR

We add a multi-part player mannequin prefab (`Assets/Characters/Player/PlayerMannequin.prefab`)
built from Unity primitive Capsule meshes arranged in a humanoid skeleton layout:

```
PlayerMannequin (root — PlayerCharacter + Animator)
└── Body
    ├── Head
    ├── Neck
    ├── Chest
    │   ├── Spine
    │   ├── Clavicle_L → UpperArm_L → Forearm_L → Hand_L
    │   └── Clavicle_R → UpperArm_R → Forearm_R → Hand_R
    └── Hips
        ├── Thigh_L → Calf_L → Foot_L
        └── Thigh_R → Calf_R → Foot_R
```

The mannequin uses Unity's built-in capsule primitive — no external assets required,
no binary blobs in the repo, fully text-diffable YAML. An `Animator` component on the
root drives a looping `PlayerIdleClip.anim` via `PlayerAnimatorController.controller`.
A `PlayerCharacter` MonoBehaviour exposes a `FaceDirection()` hook for future locomotion.

The body-part hierarchy mirrors the standard humanoid bone layout so a future
SIGNALIS-styled skinned mesh can be retargeted onto the same tree without modifying
the controller or animation graph.

**Engine correction (vs earlier draft):** An earlier draft implemented this in Unreal
Engine 5. The issue repo uses **Unity** (per `GAME_DESIGN.md` — "Движок: Unity"), so
this implementation targets Unity 6.3 LTS, consistent with PR #8 which migrates the
project skeleton to Unity.
