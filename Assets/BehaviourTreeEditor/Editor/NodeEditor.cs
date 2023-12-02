using BehaviourTreeEditor.BTree;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Node))]
    public class NodeEditor : UnityEditor.Editor
    {
        // public override void OnInspectorGUI()
        // {
        //     // Cast the target to your node type
        //     Node node = (Node)target;
        //
        //     // Draw the default inspector
        //     DrawDefaultInspector();
        //
        //     // Create a dropdown for selecting GameObjects
        //     EditorGUI.BeginChangeCheck();
        //     GameObject newTarget = (GameObject)EditorGUILayout.ObjectField("Target GameObject", node.targetGameObject, typeof(GameObject), true);
        //     if (EditorGUI.EndChangeCheck())
        //     {
        //         Undo.RecordObject(node, "Change Target GameObject");
        //         node.targetGameObject = newTarget;
        //         EditorUtility.SetDirty(node);
        //     }
        // }
    }
}