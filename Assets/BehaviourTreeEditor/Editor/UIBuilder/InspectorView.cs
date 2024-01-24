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

        public void UpdateSelectedNodeInspector(NodeView nodeView)
        {
            Clear();
            Object.DestroyImmediate(editor);
            if (nodeView == null)
            {
                // todo: when nodeView is null, show the some tips in the inspector view
                return;
            }

            editor = UnityEditor.Editor.CreateEditor(nodeView.node);

            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor && editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}