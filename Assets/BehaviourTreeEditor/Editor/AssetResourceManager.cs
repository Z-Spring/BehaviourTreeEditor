﻿using System.Linq;
using UnityEngine;

namespace Editor
{
    public static class AssetResourceManager
    {
        const string RootBehaviourTreeEditorPath = "Assets/BehaviourTreeEditor/Profiles";
        public const string BTEditorUXMLPath = "Assets/BehaviourTreeEditor/Editor/UIBuilder/BTreeEditor.uxml";
        public const string BTEditorUSSPath = "Assets/BehaviourTreeEditor/Editor/UIBuilder/BTreeEditor.uss";
        public const string NodeViewUXMLPath = "Assets/BehaviourTreeEditor/Editor/UIBuilder/NodeView.uxml";
        
        public const string IconPath = "Assets/BehaviourTreeEditor/Gizmos/Icon.png";
        public const string DeleteIconPath = "Assets/BehaviourTreeEditor/Gizmos/Delete2.png";
        public const string PlayIconPath = "Assets/BehaviourTreeEditor/Gizmos/Play.png";
        public const string PauseIconPath = "Assets/BehaviourTreeEditor/Gizmos/Pause.png";
        public const string StepIconPath = "Assets/BehaviourTreeEditor/Gizmos/Step.png";
        public static string GetSharedVariableContainerAssetPath(string treeName)
        {
            return $"{RootBehaviourTreeEditorPath}/{treeName}_SharedVariableContainer.asset";
        }

        public static string GetBehaviourTreeAssetPath(string treeName)
        {
            return $"{RootBehaviourTreeEditorPath}/{treeName}.asset";
        }

        public static void CreateScriptObjectAsset<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(asset, path);
        }

        public static T LoadAsset<T>(string path) where T : Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static T[] LoadAllAssets<T>(string path) where T : Object
        {
            return UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path).OfType<T>().ToArray();
        }

        public static void CreateAsset(Object asset, string path)
        {
            UnityEditor.AssetDatabase.CreateAsset(asset, path);
        }

        public static void SaveAsset(Object asset)
        {
            UnityEditor.EditorUtility.SetDirty(asset);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
}