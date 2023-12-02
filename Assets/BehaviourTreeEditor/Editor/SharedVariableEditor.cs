using System.CodeDom;
using System.Reflection;
using BehaviourTreeEditor.SharedVariables;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SharedVariable), true)]
    public class SharedVariableEditor : UnityEditor.Editor
    {
        private FieldInfo[] fields;
        SharedVariable myTarget;
        ScriptableObject so;
        private string label;

        private void OnEnable()
        {
            fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (target == null)
            {
                return;
            }

            myTarget = (SharedVariable)target;
            so = (ScriptableObject)myTarget;
        }

        public override void OnInspectorGUI()
        {
            foreach (var field in fields)
            {
                label = $"{so.name} ({field.FieldType.Name})";

                if (so != null)
                {
                    if (field.FieldType == typeof(bool))
                    {
                        bool currentValue = (bool)field.GetValue(myTarget);
                        bool newValue = EditorGUILayout.Toggle(label, currentValue);
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        string newValue = EditorGUILayout.TextField(label,
                            field.GetValue(myTarget)?.ToString());
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        int currentValue = (int)field.GetValue(myTarget);
                        int newValue = EditorGUILayout.IntField(label, currentValue);
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        float currentValue = (float)field.GetValue(myTarget);
                        float newValue = EditorGUILayout.FloatField(label, currentValue);
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(Vector2))
                    {
                        Vector2 currentValue = (Vector2)field.GetValue(myTarget);
                        Vector2 newValue = EditorGUILayout.Vector2Field(label, currentValue);
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(Vector3))
                    {
                        Vector3 currentValue = (Vector3)field.GetValue(myTarget);
                        Vector3 newValue = EditorGUILayout.Vector3Field(label, currentValue);
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(Color))
                    {
                        Color currentValue = (Color)field.GetValue(myTarget);
                        Color newValue = EditorGUILayout.ColorField(label, currentValue);
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(AnimationCurve))
                    {
                        AnimationCurve currentValue = (AnimationCurve)field.GetValue(myTarget);
                        AnimationCurve newValue = EditorGUILayout.CurveField(label, currentValue);
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(Bounds))
                    {
                        Bounds currentValue = (Bounds)field.GetValue(myTarget);
                        Bounds newValue = EditorGUILayout.BoundsField(label, currentValue);
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(Rect))
                    {
                        Rect currentValue = (Rect)field.GetValue(myTarget);
                        Rect newValue = EditorGUILayout.RectField(label, currentValue);
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(Quaternion))
                    {
                        Quaternion currentValue = (Quaternion)field.GetValue(myTarget);
                        Quaternion newValue =
                            Quaternion.Euler(EditorGUILayout.Vector3Field(label, currentValue.eulerAngles));
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(GameObject))
                    {
                        GameObject currentValue = (GameObject)field.GetValue(myTarget);
                        GameObject newValue =
                            EditorGUILayout.ObjectField(label, currentValue, typeof(GameObject), true) as GameObject;
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(Transform))
                    {
                        Transform currentValue = (Transform)field.GetValue(myTarget);
                        Transform newValue =
                            EditorGUILayout.ObjectField(label, currentValue, typeof(Transform), true) as Transform;
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(Texture2D))
                    {
                        Texture2D currentValue = (Texture2D)field.GetValue(myTarget);
                        Texture2D newValue =
                            EditorGUILayout.ObjectField(label, currentValue, typeof(Texture2D), true) as Texture2D;
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(Sprite))
                    {
                        Sprite currentValue = (Sprite)field.GetValue(myTarget);
                        Sprite newValue =
                            EditorGUILayout.ObjectField(label, currentValue, typeof(Sprite), true) as Sprite;
                        field.SetValue(myTarget, newValue);
                    }
                    else if (field.FieldType == typeof(Material))
                    {
                        Material currentValue = (Material)field.GetValue(myTarget);
                        Material newValue =
                            EditorGUILayout.ObjectField(label, currentValue, typeof(Material), true) as Material;
                        field.SetValue(myTarget, newValue);
                    }
                }

                // 获取字段的值
                // 对于 bool 型字段
            }

            // Save changes to the SharedVariable
            EditorUtility.SetDirty(target);
        }
    }
}