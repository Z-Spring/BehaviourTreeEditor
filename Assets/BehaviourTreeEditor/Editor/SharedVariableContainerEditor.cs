using BehaviourTreeEditor.BTree.ParentSharedVariable;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(SharedVariableContainer))]
    public class SharedVariableContainerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, new[] { "m_Script" });
        }
    }
}