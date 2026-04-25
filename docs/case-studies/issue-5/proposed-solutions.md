# Proposed solutions — Issue #5: Player Model

## Options considered (Unity 6.3 LTS)

| # | Approach | Binary assets? | CI-compatible | Effort | Verdict |
|---|---|---|---|---|---|
| **A** | **Unity primitive-capsule hierarchy prefab** | **No** | **✅** | **Low** | **Selected** |
| B | Mixamo FBX humanoid (Y Bot / Adam) + idle retarget | FBX + textures | ✅ (Git LFS) | Medium | Deferred |
| C | URP Starter Assets (Unity Technologies) | Partial | ✅ | Medium | Overkill for mannequin |
| D | ProBuilder proto mesh | After editor bake | ✅ | Medium | Requires extra package |
| E | Runtime-generated procedural mesh (C# only) | No | ✅ | High | Overengineered |

## Why A wins for this issue

1. **Zero binary assets in the diff.** The PR is exclusively text: YAML prefab,
   YAML animation clip, YAML animator controller, C# scripts, Markdown. Every
   byte is reviewable.
2. **Built-in primitive.** Unity's Capsule mesh (`fileID: 10208`,
   `guid: 0000000000000000e000000000000000`) is available in every project; no
   package install, no FBX import, no editor-import step.
3. **Correct multi-part hierarchy.** 13 segments cover all required joints:
   clavicles, upper arms, forearms, hands; thighs, calves, feet (R3).
4. **CI-compatible.** GameCI `unity-builder@v4` can package the prefab headlessly
   without launching the editor interactively.
5. **SIGNALIS-friendly.** Low-poly capsule proportions match the chunky PS1-era
   silhouette; future swap is just replacing segment meshes (R4).

## Files delivered

| File | Purpose |
|---|---|
| `Assets/Characters/Player/PlayerMannequin.prefab` | 13-segment humanoid capsule mannequin with `PlayerCharacter` + `Animator`. |
| `Assets/Characters/Player/Animations/PlayerIdleClip.anim` | 2-second looping breathing idle (chest Y oscillation ±3 cm at 60 fps). |
| `Assets/Characters/Player/Animations/PlayerAnimatorController.controller` | Single-state Animator Controller: enters Idle state on trigger. |
| `Assets/Characters/Player/Scripts/PlayerCharacter.cs` | `MonoBehaviour`: plays Idle on Start; exposes `FaceDirection()` for future locomotion. |
| `Assets/Characters/Player/Scripts/PlayerEditorSetup.cs` | Editor-only menu item: **GameObject → One-try → Add Player Mannequin**. |

## Existing Unity components reused

| Component | Source | Role |
|---|---|---|
| Capsule primitive mesh | Unity built-in | All 13 body segments — no external assets. |
| `Animator` | Unity built-in (`UnityEngine.Animator`) | Drives the Idle animation on the prefab root. |
| `MeshFilter` + `MeshRenderer` | Unity built-in | Renders each capsule segment. |
| Default material | Unity built-in (`fileID: 10303`) | Grey diffuse placeholder; easy to swap. |

No third-party plugins, no Marketplace/Asset Store dependencies, no network downloads.

## SIGNALIS next step (out of scope for this issue)

When a low-poly Replika mesh is ready:
1. Skin it to a Humanoid avatar rig in Blender (or retarget via Unity's Humanoid pipeline).
2. Import the FBX, set rig type to **Humanoid** in Import Settings.
3. Reassign the `Animator.Avatar` on `PlayerMannequin` to the new Humanoid avatar.
4. `PlayerCharacter.cs`, `PlayerAnimatorController`, and the CI workflow are unchanged.

Alternatively, keep the capsule tree and add a skinned mesh as a child of `Body/` with
bones that bind to the existing Transform hierarchy.
