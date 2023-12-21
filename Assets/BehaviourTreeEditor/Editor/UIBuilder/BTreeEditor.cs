using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTreeEditor.BTree;
using BehaviourTreeEditor.BTree.ParentSharedVariable;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace Editor
{
    public class BTreeEditor : EditorWindow
    {
        BtreeView treeView;
        InspectorView inspectorView;
        CreateSharedVariableEditor createSharedVariableEditor;
        BehaviourTreeRunnerEditor behaviourTreeRunnerEditor;
        VisualElement root;
        ToolbarButton deleteRunnerButton;
        ToolbarButton addRunnerButton;
        SharedVariableContainer sharedVariableContainer;

        static BehaviourTree tree;
        public static List<Type> nodeTypes = new();
        public static string selectedTreeName;


        private void OnEnable()
        {
            GetNodeTypes();

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        [MenuItem("BTreeEditor/Show Window")]
        public static void OpenWindow()
        {
            var window = GetWindow<BTreeEditor>();
            window.minSize = new Vector2(800, 600);
        }

        public void CreateGUI()
        {
            root = rootVisualElement;

            LoadUIAssets();
            InitializeEditorWindowElements();
            AddBehaviourTreeRunnerComponent();
            DeleteBehaviourTreeRunnerComponent();
            treeView.OnNodeSelected = OnNodeSelectionChanged;

            string treeName = GetCurrentTreeName();
            createSharedVariableEditor = new CreateSharedVariableEditor();
            UpdateCurrentSharedVariableView(treeName);
            createSharedVariableEditor.CreateSharedVariableDropdownField(root);
            createSharedVariableEditor.AddVariable(root, sharedVariableContainer);
            OnSelectionChange();
        }

        void LoadUIAssets()
        {
            var visualTree = AssetResourceManager.LoadAsset<VisualTreeAsset>(AssetResourceManager.BTEditorUXMLPath);
            visualTree.CloneTree(root);

            var styleSheet =
                AssetResourceManager.LoadAsset<StyleSheet>(AssetResourceManager.BTEditorUSSPath);
            root.styleSheets.Add(styleSheet);
        }

        void InitializeEditorWindowElements()
        {
            treeView = root.Q<BtreeView>();
            inspectorView = root.Q<InspectorView>();
            deleteRunnerButton = root.Q<ToolbarButton>("DeleteRunner");
            addRunnerButton = root.Q<ToolbarButton>("AddRunner");

            root.Q<Button>("SaveAsset").RegisterCallback<ClickEvent>(evt => treeView.SaveNodeAsset());
        }

        static string GetCurrentTreeName()
        {
            return Selection.activeGameObject == null
                ? null
                : Selection.activeGameObject.TryGetComponent(out BehaviourTreeRunner behaviourTreeRunner)
                    ? behaviourTreeRunner.treeName
                    : null;
        }

        void UpdateCurrentSharedVariableView(string treeName)
        {
            sharedVariableContainer = createSharedVariableEditor.LoadSharedVariableContainerAsset(treeName);
            createSharedVariableEditor.InitializeVariableEditors(treeName, root);
        }

        void AddBehaviourTreeRunnerComponent()
        {
            addRunnerButton.tooltip = "Add Behaviour Tree Runner Component";
            addRunnerButton.clicked += () =>
            {
                var runner = Selection.activeGameObject;
                if (runner.GetComponent<BehaviourTreeRunner>())
                {
                    return;
                }

                var behaviourTreeRunner = runner.AddComponent<BehaviourTreeRunner>();
                InitBehaviourTree(behaviourTreeRunner);
                OnSelectionChange();
            };
        }

        void InitBehaviourTree(BehaviourTreeRunner behaviourTreeRunner)
        {
            if (behaviourTreeRunner.tree == null)
            {
                behaviourTreeRunner.tree = CreateInstance<BehaviourTree>();
                if (Selection.activeObject is GameObject gameObject)
                {
                    behaviourTreeRunner.treeName = $"{gameObject.name}";
                    behaviourTreeRunner.tree.name = behaviourTreeRunner.treeName;
                }

                BehaviourTreeRunnerEditor.previousTreeName = behaviourTreeRunner.treeName;
                PlayerPrefs.SetString("previousTreeName", BehaviourTreeRunnerEditor.previousTreeName);
                AssetDatabase.CreateAsset(behaviourTreeRunner.tree, $"Assets/{behaviourTreeRunner.treeName}.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void DeleteBehaviourTreeRunnerComponent()
        {
            deleteRunnerButton.tooltip = "Delete Behaviour Tree Runner Component";
            deleteRunnerButton.clicked += () =>
            {
                var runner = Selection.activeGameObject;
                BehaviourTreeRunner behaviourTreeRunner = runner.GetComponent<BehaviourTreeRunner>();
                if (behaviourTreeRunner)
                {
                    if (behaviourTreeRunner.tree == null)
                    {
                        DestroyImmediate(behaviourTreeRunner);
                        return;
                    }

                    string treeName = behaviourTreeRunner.tree.name;
                    treeView.ResetView();
                    root.Query<Label>("BehaviourTreeName").First().text = "Click + To Add A Behaviour Tree Runner";
                    DeleteRelatedAssets(behaviourTreeRunner, treeName);
                }
            };
        }

        void DeleteRelatedAssets(BehaviourTreeRunner behaviourTreeRunner, string treeName)
        {
            string treeAssetPath = AssetDatabase.GetAssetPath(behaviourTreeRunner.tree);
            string sharedVariableContainerAssetPath =
                AssetResourceManager.GetSharedVariableContainerAssetPath(treeName);
            AssetDatabase.DeleteAsset(sharedVariableContainerAssetPath);
            AssetDatabase.DeleteAsset(treeAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            DestroyImmediate(behaviourTreeRunner);
        }

        void OnSelectionChange()
        {
            if (treeView == null)
            {
                return;
            }

            if (Selection.activeGameObject)
            {
                var runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if (runner)
                {
                    // todo: inspectorView.PopulateView(null);
                    tree = runner.tree;
                    selectedTreeName = runner.treeName;

                    treeView.PopulateView(tree);
                    treeView.AutoFrameNode();

                    UpdateCurrentSharedVariableView(selectedTreeName);
                }
            }

            // todo : it is necessary to add this if statement?
            if (Application.isPlaying)
            {
                if (tree)
                {
                    treeView.PopulateView(tree);
                }
            }
            else
            {
                // when select a treeAsset, display tree
                if (tree != null && /*todo: dont understand*/
                    AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    treeView.PopulateView(tree);
                }
            }
        }


        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (Selection.activeObject is BehaviourTree)
            {
                Debug.Log(Selection.activeObject.name);
                OpenWindow();
                return true;
            }

            return false;
        }


        void GetNodeTypes()
        {
            nodeTypes.Clear();
            nodeTypes = typeof(Node).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Node)))
                .ToList();
        }

        void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        void OnNodeSelectionChanged(NodeView node)
        {
            inspectorView.UpdateSelectedNodeInspector(node);
        }

        void OnInspectorUpdate()
        {
            treeView?.UpdateNodeState();
        }

        #region save changes before quitting

        private void OnDestroy()
        {
            if (treeView.HasUnsavedChanges)
            {
                treeView.SaveNodeAsset();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion
    }
}