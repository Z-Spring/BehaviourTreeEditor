using DefaultNamespace;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SetPatrolPoints))]
    public class SetPatrolPointEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Tile Editor", EditorStyles.boldLabel);
            string[] editDateStr = { "Dead", "Alive", "Guard" };
            int s = 0;
            s = GUILayout.Toolbar(s,editDateStr);
            DrawDefaultInspector();
        }
    }
}