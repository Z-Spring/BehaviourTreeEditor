using System;
using System.Reflection;
using BehaviourTreeEditor.BTree.SharedVariables;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor
{
    [CustomEditor(typeof(SharedVariable), true)]
    public class SharedVariableEditor : UnityEditor.Editor
    {
        FieldInfo[] fields;
        SharedVariable myTarget;
        ScriptableObject so;
        string label;
        Texture2D iconTexture;
        EditorWindow MyEditorWindow => EditorWindow.GetWindow<BTreeEditor>();
        ScrollView scrollView;

        //todo: why here can use Awake method?
        private void Awake()
        {
            scrollView = MyEditorWindow.rootVisualElement.Q<ScrollView>();
        }


        private void OnEnable()
        {
            fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (target == null)
            {
                return;
            }

            myTarget = (SharedVariable)target;
            so = (ScriptableObject)myTarget;
            iconTexture =
                EditorGUIUtility.Load("Assets/BehaviourTreeEditor/Gizmos/Delete2.png") as Texture2D;
        }

        public override void OnInspectorGUI()
        {
            if (so == null || myTarget == null)
            {
                return;
            }

            var field = fields[0];

            label = $"{so.name} ({field.FieldType.Name})";
            EditorGUILayout.BeginHorizontal();

            object currentValue = field.GetValue(myTarget);
            object newValue = null;

            switch (field.FieldType.Name)
            {
                case "Boolean":
                    newValue = EditorGUILayout.Toggle(label, (bool)currentValue);
                    break;
                case "Int32":
                    newValue = EditorGUILayout.IntField(label, (int)currentValue);
                    break;
                case "String":
                    newValue = EditorGUILayout.TextField(label, (string)currentValue);
                    break;
                case "Single":
                    newValue = EditorGUILayout.FloatField(label, (float)currentValue);
                    break;

                case "Vector2":
                    newValue = EditorGUILayout.Vector2Field(label, (Vector2)currentValue);
                    break;
                case "Vector3":
                    newValue = EditorGUILayout.Vector3Field(label, (Vector3)currentValue);
                    break;
                case "Quaternion":
                    newValue = EditorGUILayout.Vector3Field(label, ((Quaternion)currentValue).eulerAngles);
                    break;

                case "Color":
                    newValue = EditorGUILayout.ColorField(label, (Color)currentValue);
                    break;
                case "Rect":
                    newValue = EditorGUILayout.RectField(label, (Rect)currentValue);
                    break;
                case "GameObject":
                    newValue = EditorGUILayout.ObjectField(label, (GameObject)currentValue, typeof(GameObject),
                        true);
                    break;
                case "Transform":
                    newValue = EditorGUILayout.ObjectField(label, (Transform)currentValue, typeof(Transform), true);
                    break;
                case "Texture2D":
                    newValue = EditorGUILayout.ObjectField(label, (Texture2D)currentValue, typeof(Texture2D), true);
                    break;
                case "Sprite":
                    newValue = EditorGUILayout.ObjectField(label, (Sprite)currentValue, typeof(Sprite), true);
                    break;
                case "Material":
                    newValue = EditorGUILayout.ObjectField(label, (Material)currentValue, typeof(Material), true);
                    break;
                case "AnimationCurve":
                    newValue = EditorGUILayout.CurveField(label, (AnimationCurve)currentValue);
                    break;
                case "Object":
                    newValue = EditorGUILayout.ObjectField(label, (Object)currentValue, typeof(Object), true);
                    break;
                case "Collider":
                    newValue = EditorGUILayout.ObjectField(label, (Collider)currentValue, typeof(Collider), true);
                    break;
                case "LayerMask":
                    newValue = EditorGUILayout.LayerField(label, (LayerMask)currentValue);
                    break;
                case "Bounds":
                    newValue = EditorGUILayout.BoundsField(label, (Bounds)currentValue);
                    break;
            }

            field.SetValue(myTarget, newValue);
            EditorUtility.SetDirty(target);
            var buttonGUIContent = new GUIContent(iconTexture);
            if (GUILayout.Button(buttonGUIContent, GUILayout.Width(20), GUILayout.Height(18)))
            {
                // newValue = string.Empty;
                if (EditorUtility.DisplayDialog("Delete variable",
                        "Are you sure you want to delete this variable?", "Yes", "No"))
                {
                    DestroyImmediate(so, true);

                    DeleteVariable(myTarget);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        void DeleteVariable(SharedVariable variable)
        {
            if (CreateSharedVariableEditor.dic.TryGetValue(variable, out var container))
            {
                CreateSharedVariableEditor.dic.Remove(variable);
                if (scrollView.Contains(container))
                {
                    scrollView.Remove(container);
                }

                SharedVariablePopupEditor.RemoveSharedVariable(variable);
            }

            DestroyImmediate(variable);
        }
    }
}