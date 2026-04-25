# References

## SIGNALIS — art direction

- [Signalis — Wikipedia](https://en.wikipedia.org/wiki/Signalis) — design intent: "replicate the graphics and gameplay of fifth-generation video games", influences from *Silent Hill* and *Resident Evil*.
- [rose-engine — Signalis press kit](https://rose-engine.org/press/sheet.php?p=signalis) — official assets and stylistic statement.

## Unity — humanoid rig & animation

- [Unity Manual: Humanoid Avatars](https://docs.unity3d.com/Manual/AvatarCreationandSetup.html) — the Humanoid pipeline that a future skinned mesh will use.
- [Unity Manual: Animation](https://docs.unity3d.com/Manual/AnimationSection.html) — Animator, AnimationClip, AnimatorController docs.
- [Unity Manual: Prefabs](https://docs.unity3d.com/Manual/Prefabs.html) — prefab YAML serialisation format.
- [Unity Discussions: How to create a humanoid mannequin in Unity without FBX](https://discussions.unity.com/t/how-to-create-a-custom-humanoid-avatar-without-using-fbx/185785) — community discussion confirming the primitive-hierarchy approach.
- [Standard Humanoid Bone Layout (Mixamo reference)](https://helpdesk.rokoko.com/hc/en-us/articles/360012783559-What-is-the-standard-bone-hierarchy-in-Mixamo) — canonical joint naming: clavicle, upperArm, foreArm, hand; thigh, leg (calf), foot.

## CI — GameCI Unity builder

- [GameCI unity-builder@v4 — GitHub](https://github.com/game-ci/unity-builder) — the action used in `.github/workflows/build.yml`.
- [GameCI Documentation](https://game.ci/docs/github/getting-started) — full setup guide for `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD` secrets.
- [GameCI unity-request-activation-file@v2 — GitHub](https://github.com/game-ci/unity-request-activation-file) — generates the `.alf` manual activation request.

## Alternative pre-built characters / libraries (considered, not used)

- [Mixamo](https://www.mixamo.com/) — free auto-rigged FBX characters + animations, good SIGNALIS-style replacement in a future PR.
- [Unity URP Starter Assets](https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526) — polished third-person character, but overkill for mannequin-only work.
- [Kenney Skeletons (CC0)](https://kenney.nl/assets/skeleton) — free low-poly humanoid; viable for SIGNALIS-adjacent look.
