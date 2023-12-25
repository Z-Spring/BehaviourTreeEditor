using BehaviourTreeEditor.BTree;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class HierarchyIcon : UnityEditor.Editor
    {
        private static Texture2D icon = AssetResourceManager.LoadAsset<Texture2D>(AssetResourceManager.IconPath);

        [InitializeOnLoadMethod]
        static void AddHierarchyIcon()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var runners = FindObjectsOfType<BehaviourTreeRunner>();
            foreach (var runner in runners)
            {
                var gameObject = runner.gameObject;
                if (gameObject.GetInstanceID() == instanceID)
                {
                    Rect position = new Rect(selectionRect);
                    position.x = position.width + (selectionRect.x - 20f);
                    position.width = 20f;
                    position.height = 18f;
                    GUI.Label(position, icon);
                }
            }
        }
    }
}