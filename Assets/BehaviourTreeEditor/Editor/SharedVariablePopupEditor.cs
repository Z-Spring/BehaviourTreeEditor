﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviourTreeEditor.BTree;
using BehaviourTreeEditor.BTree.SharedVariables;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Node), true)]
    [CanEditMultipleObjects]
    public class SharedVariablePopupEditor : UnityEditor.Editor
    {
        static List<ScriptableObject> allSharedVariableScriptableObjects;
        static string currentTreeName;

        bool IsTreeNameChanged()
        {
            if (currentTreeName != BTreeEditor.selectedTreeName)
            {
                currentTreeName = BTreeEditor.selectedTreeName;
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            if (IsTreeNameChanged())
            {
                allSharedVariableScriptableObjects = GetAllSharedVariableScriptableObjects();
            }
        }

        public override void OnInspectorGUI()
        {
            var fields = GetCurrentNodeSharedVariableFields().ToList();
            DrawExcludeSharedVariables(fields);
            InitSharedVariableFields(fields);
        }

        List<ScriptableObject> GetAllSharedVariableScriptableObjects()
        {
            var sharedVariableContainerAssetPath =
                AssetResourceManager.GetSharedVariableContainerAssetPath(BTreeEditor.selectedTreeName);
            var allSharedVariableAssets =
                AssetResourceManager.LoadAllAssets<ScriptableObject>(sharedVariableContainerAssetPath);

            allSharedVariableScriptableObjects = allSharedVariableAssets.ToList();
            return allSharedVariableScriptableObjects;
        }

        IEnumerable<FieldInfo> GetCurrentNodeSharedVariableFields()
        {
            var nodeType = target.GetType();
            return nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(field => typeof(SharedVariable).IsAssignableFrom(field.FieldType) ||
                                field.FieldType == typeof(SharedVariable));
        }

        void InitSharedVariableFields(List<FieldInfo> sharedVariableFields)
        {
            foreach (var field in sharedVariableFields)
            {
                var scriptableObjects = allSharedVariableScriptableObjects
                    .Where(scriptableObject => scriptableObject.GetType() == field.FieldType ||
                                               scriptableObject.GetType().IsSubclassOf(field.FieldType)).ToArray();
                var names = scriptableObjects.Select(scriptableObject => scriptableObject.name).ToArray();
                DrawSharedVariablePopup(names, ref scriptableObjects, field);
            }
        }

        void DrawSharedVariablePopup(string[] names, ref ScriptableObject[] scriptableObjects, FieldInfo field)
        {
            var sharedVariable = (SharedVariable)field.GetValue(target);

            int selectedIndex = sharedVariable != null ? Array.IndexOf(names, sharedVariable.name) : -1;

            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUILayout.Popup(ObjectNames.NicifyVariableName(field.Name), selectedIndex,
                names);
            if (EditorGUI.EndChangeCheck() && selectedIndex >= 0)
            {
                var selectedScriptableObject = scriptableObjects[selectedIndex];
                field.SetValue(target, selectedScriptableObject);
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawExcludeSharedVariables(List<FieldInfo> fields)
        {
            var propertyNamesToExclude = new[] { "m_Script" }
                .Concat(fields.Select(field => field.Name)).ToArray();

            DrawPropertiesExcluding(serializedObject, propertyNamesToExclude);
            serializedObject.ApplyModifiedProperties();
        }

        public static void AddSharedVariable(SharedVariable variable)
        {
            if (allSharedVariableScriptableObjects == null)
            {
                allSharedVariableScriptableObjects = new List<ScriptableObject>();
            }

            allSharedVariableScriptableObjects.Add(variable);
        }

        public static void RemoveSharedVariable(SharedVariable variable)
        {
            allSharedVariableScriptableObjects.Remove(variable);
        }
    }
}