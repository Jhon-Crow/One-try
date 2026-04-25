# Analysis — Player Model for One-try (Unity 6.3 LTS)

## 1. Project context

The repository is a **Unity 6.3 LTS** project (`ProjectSettings/ProjectVersion.txt:
m_EditorVersion: 6000.3.5f1`). Per `GAME_DESIGN.md`: "**Движок: Unity**".

An earlier draft of this PR (before the owner's feedback) implemented the mannequin
in Unreal Engine 5, which was wrong. This analysis reflects the corrected Unity
implementation.

Previous PRs established:

- A clean Unity project skeleton: `Assets/Scenes/SampleScene.unity`, `Packages/manifest.json`,
  `ProjectSettings/ProjectVersion.txt`.
- A working CI workflow (`build.yml`) using GameCI `unity-builder@v4` to cross-compile
  to `StandaloneWindows64`.

No gameplay assets existed yet — no player character, no movement, no animation.

## 2. Why a primitive-capsule mannequin (no external asset import)

The issue authorises using presets ("используй пресеты если необходимо"), but Unity
has no built-in "Third Person Character" template that ships with a standalone humanoid
mannequin the way UE5 does.

Options considered:

| Approach | Pros | Cons |
|---|---|---|
| Unity Humanoid default avatar (Cinemachine + Mixamo FBX) | Industry standard, retarget-ready | Requires binary FBX assets (~MB), licence questions, no playable baseline without the editor |
| Primitive capsule hierarchy | Zero deps, fully text-diffable YAML, no editor needed | Not a skinned mesh; cosmetically coarser |
| URP starter assets (Unity Technologies) | Polished, includes animations | Large dependency, changes manifest |
| ProBuilder proto mesh | Precise geometry | Adds ProBuilder package, still binary after first bake |
| Runtime-generated procedural mesh | 100% code | Complex, hard to animate |

**Selected:** primitive-capsule hierarchy. Reasons:

1. **Text-only diff** — the prefab YAML is 100% reviewable; no opaque binary blobs.
2. **Zero extra dependencies** — uses Unity built-in `Capsule` mesh (`fileID: 10208`,
   `guid: 0000000000000000e000000000000000`), available in every Unity project.
3. **Correct multi-part hierarchy** — the 13-segment tree satisfies R3 exactly
   (clavicles, upper arms, forearms, hands; thighs, calves, feet).
4. **SIGNALIS-ready** — low-poly primitive silhouette resembles the PS1-era aesthetic
   of Signalis; future swap is just replacing the mesh per segment or switching to a
   full skinned mesh on the same bone tree.
5. **CI-compatible** — GameCI headless build can package this prefab immediately.

## 3. Body hierarchy

The `PlayerMannequin.prefab` hierarchy, reflecting a standard humanoid joint chain:

```
PlayerMannequin (root)          ← PlayerCharacter.cs + Animator
└── Body
    ├── Head
    ├── Neck
    ├── Chest
    │   ├── Spine              (connector between hips and chest)
    │   ├── Clavicle_L
    │   │   └── UpperArm_L
    │   │       └── Forearm_L
    │   │           └── Hand_L
    │   └── Clavicle_R
    │       └── UpperArm_R
    │           └── Forearm_R
    │               └── Hand_R
    └── Hips
        ├── Thigh_L
        │   └── Calf_L
        │       └── Foot_L
        └── Thigh_R
            └── Calf_R
                └── Foot_R
```

All segments use Unity's built-in Capsule primitive with `MeshFilter` + `MeshRenderer`
(default material). The parent–child transform hierarchy encodes the joint positions
and segment sizes: `localPosition` places the segment pivot at the joint; `localScale`
sets the capsule radius/length to match human proportions at 1.75 m standing height.

## 4. Animation

A `PlayerAnimatorController.controller` contains a single Idle state wired to
`PlayerIdleClip.anim`, which animates a gentle breathing oscillation (±3 cm on the
Chest's Y position over a 2-second loop). The Animator is attached to the prefab root.

`PlayerCharacter.cs` calls `animator.SetTrigger("Idle")` on `Start()` so the idle
loop begins immediately when the scene loads.

## 5. SIGNALIS aesthetic — what we're aiming at

From the rose-engine press kit and reviews:

- SIGNALIS deliberately replicates **fifth-generation (PS1-era) survival horror**
  rendering — low-poly 3D characters, limited texture resolution, 2D sprites and
  dithered post-processing.
- Character silhouette: humanoid Replika (worker-android) — slim, uniformed, clearly
  readable at low resolution.
- Lighting: low-key, high-contrast, monochrome accents over muted reds/teals.

**Implication:** the capsule-based mannequin is a good stand-in; it has the chunky
PSX-era proportions. The future SIGNALIS pass:

1. Replace each capsule's material with a flat/cel-shaded master material.
2. Or replace the entire prefab hierarchy with a skinned mesh (FBX + Humanoid rig)
   using the same Animator Controller — `PlayerCharacter.cs` and the CI workflow are
   untouched.

## 6. Constraints imposed by the build environment

- **No Unity Editor in CI.** GameCI's `unity-builder@v4` runs headless; it can import
  and build Unity YAML assets (prefabs, scenes, animators) without interactive input.
- **Binary assets cannot be hand-authored.** FBX, PNG, and AudioClip files require the
  editor to import. The capsule primitive bypasses this completely.
- **Unity YAML is plain text.** Prefabs and AnimationClips are YAML-serialized when the
  project uses text-mode serialization (`ProjectSettings/EditorSettings.asset` default).

## 7. Validation plan

Manual (in editor):

1. Open the project in Unity 6.3 LTS via Unity Hub.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Via **GameObject → One-try → Add Player Mannequin** (or drag the prefab from
   `Assets/Characters/Player/PlayerMannequin.prefab`).
4. Press **Play**. Expect: multi-part mannequin visible; chest gently bobs; Animator
   shows "Idle" state active.

Automated (CI):

- GameCI `unity-builder@v4` packages the scene including the prefab. The build
  succeeds as long as `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD` secrets are
  set in the repository (see README).

## 8. Risks & mitigations

| Risk | Mitigation |
|---|---|
| `PlayerMannequin.prefab` opens in wrong Unity version and capsule fileIDs differ | Built-in primitive GUIDs (`0000000000000000e000000000000000`) are stable across all Unity versions. |
| Future SIGNALIS mesh swap needs a different hierarchy | `PlayerCharacter.cs` only requires an `Animator` on root; hierarchy under `Body` can be completely replaced. |
| Idle animation only animates Chest, other segments static | Sufficient for mannequin verification; future PRs extend the AnimatorController. |
