using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class InspectorView : VisualElement
    {
        private UnityEditor.Editor editor;

        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
        {
        }

        public InspectorView()
        {
        }

        public void PopulateView(NodeView nodeView)
        {
            Clear();
            Object.DestroyImmediate(editor);
            if (nodeView == null)
            {
                return;
            }
            editor = UnityEditor.Editor.CreateEditor(nodeView.node);
            if (editor == null)
            {
                Debug.LogError("Failed to create editor for nodeView.node");
                return;
            }

            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}