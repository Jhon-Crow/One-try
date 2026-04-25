# Assets

This is the root of the Unity project assets.

The `Scenes/SampleScene.unity` file is an empty placeholder kept under
version control so that headless builds (and CI) have at least one scene
to package. Replace it via the Unity Editor — open the project, open the
scene, edit, save. Unity will rewrite the YAML to its canonical form on
the first save.

Subsequent feature work belongs in:

- `Scripts/` — gameplay, controllers, state machines.
- `Prefabs/` — reusable GameObject prefabs.
- `Art/` — meshes, textures, materials.
- `Audio/` — clips and mixers.

These subfolders are intentionally not pre-created; let them appear
alongside the first asset that needs them so the repo doesn't carry
empty directories.
