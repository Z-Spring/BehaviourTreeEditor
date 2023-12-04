﻿using BehaviourTreeEditor.BTree;
using MurphyEditor.BTree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(BehaviourTreeRunner))]
    public class BehaviourTreeRunnerEditor : UnityEditor.Editor
    {
        SerializedProperty treeName;
        SerializedProperty treeState;
        GUISkin skin;
        BehaviourTreeRunner behaviourTreeRunner;
        public static string previousTreeName;

        EditorWindow editorWindow => EditorWindow.GetWindow<BTreeEditor>();
        // BTreeEditor editorWindow => CreateInstance<BTreeEditor>();

        private void OnEnable()
        {
            treeName = serializedObject.FindProperty("treeName");
            treeState = serializedObject.FindProperty("treeState");
            behaviourTreeRunner = target as BehaviourTreeRunner;
            previousTreeName = PlayerPrefs.GetString("previousTreeName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // EditorGUILayout.PropertyField(tree);
            EditorGUILayout.PropertyField(treeName);
            EditorGUILayout.PropertyField(treeState);
            serializedObject.ApplyModifiedProperties();

            GUILayout.Label("TreeDescription", EditorStyles.boldLabel);
            behaviourTreeRunner.treeDescription =
                EditorGUILayout.TextArea(behaviourTreeRunner.treeDescription, GUILayout.MinHeight(60));
            InitBehaviourTree();
            // ChangeLabelName();
            AddButton();
        }

        void AddButton()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open Behaviour Tree Editor", GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true), GUILayout.MaxWidth(300)))
            {
                behaviourTreeRunner.tree.name = behaviourTreeRunner.treeName;
                CheckIfTreeNameChanged();
                BTreeEditor.OpenWindow();
                ChangeLabelName();
            }
            //

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        void InitBehaviourTree()
        {
            if (behaviourTreeRunner.tree == null)
            {
                behaviourTreeRunner.tree = CreateInstance<BehaviourTree>();
                if (Selection.activeObject is GameObject gameObject)
                {
                    behaviourTreeRunner.treeName = $"{gameObject.name}";
                    behaviourTreeRunner.tree.name = behaviourTreeRunner.treeName;
                }
                
                previousTreeName = behaviourTreeRunner.treeName;
                PlayerPrefs.SetString("previousTreeName", previousTreeName);
                AssetDatabase.CreateAsset(behaviourTreeRunner.tree, $"Assets/{behaviourTreeRunner.treeName}.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void CheckIfTreeNameChanged()
        {
            if (behaviourTreeRunner.treeName != previousTreeName)
            {
                if (AssetDatabase.LoadAssetAtPath<BehaviourTree>($"Assets/{previousTreeName}.asset") != null)
                {
                    AssetDatabase.RenameAsset($"Assets/{previousTreeName}.asset", behaviourTreeRunner.treeName);
                }

                previousTreeName = behaviourTreeRunner.treeName;
                PlayerPrefs.SetString("previousTreeName", previousTreeName);
            }
        }

        void ChangeLabelName()
        {
            editorWindow.rootVisualElement.Query<Label>("BehaviourTreeName").First().text =
                behaviourTreeRunner.treeName + "- Behaviour";
        }
    }
}
#endif