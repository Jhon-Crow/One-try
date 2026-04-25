#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace OneTry.Characters.Editor
{
    /// <summary>
    /// Convenience menu item to drop a PlayerMannequin into the open scene for
    /// quick iteration — no need to find the prefab in the Project window.
    /// </summary>
    public static class PlayerEditorSetup
    {
        private const string PrefabGuid = "d1e2f3a4b5c6d7e8f9a0b1c2d3e4f5a6";
        private const string MenuPath = "GameObject/One-try/Add Player Mannequin";

        [MenuItem(MenuPath, priority = 10)]
        public static void AddPlayerMannequin()
        {
            string path = AssetDatabase.GUIDToAssetPath(PrefabGuid);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError(
                    "[PlayerEditorSetup] PlayerMannequin.prefab not found. " +
                    "Make sure Assets/Characters/Player/PlayerMannequin.prefab exists.");
                return;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[PlayerEditorSetup] Failed to load prefab at: {path}");
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = Vector3.zero;
            Undo.RegisterCreatedObjectUndo(instance, "Add Player Mannequin");
            Selection.activeGameObject = instance;
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateAddPlayerMannequin()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded;
        }
    }
}
#endif
