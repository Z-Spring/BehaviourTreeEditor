using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviourTreeEditor.BTree;
using BehaviourTreeEditor.BTree.ActionNodes;
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
        public static List<Type> nodeTypes = new();
        public static string selectedTreeName;


        BtreeView treeView;
        InspectorView inspectorView;
        ScrollView scrollView;
        ToolbarMenu selectTreeToolbar;
        CreateSharedVariableEditor createSharedVariableEditor;
        BehaviourTreeRunnerEditor behaviourTreeRunnerEditor;
        VisualElement root;
        ToolbarButton deleteRunnerButton;
        ToolbarButton addRunnerButton;
        Button addVariableButton;
        TextField inputSharedValueName;
        SharedVariableContainer sharedVariableContainer;
        Texture2D icon;
        static BehaviourTree tree;
        readonly List<GameObject> theGameObjectHasRunnerList = new();

        private void OnEnable()
        {
            GetNodeTypes();
            SearchBehaviourTreeRunner();
            icon = AssetResourceManager.LoadAsset<Texture2D>(AssetResourceManager.IconPath);
            BtreeView.OnCreateTree += AddRunnerComponent;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }


        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            BtreeView.OnCreateTree -= AddRunnerComponent;
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
            AssignTreeDropDown();
            DeleteBehaviourTreeRunnerComponent();
            treeView.OnNodeSelected = OnNodeSelectionChanged;
            string treeName = GetCurrentTreeName();
            // createSharedVariableEditor = new CreateSharedVariableEditor();
            createSharedVariableEditor = CreateInstance<CreateSharedVariableEditor>();
            UpdateCurrentSharedVariableView(treeName);
            createSharedVariableEditor.CreateSharedVariableDropdownField(root);
            if (sharedVariableContainer != null)
            {
                createSharedVariableEditor.AddVariable(root, sharedVariableContainer, treeName);
            }

            addVariableButton.SetEnabled(false);
            inputSharedValueName.RegisterValueChangedCallback((evt) => { AddSharedVariableButtonStatusChange(evt.newValue); });
            OnSelectionChange();
        }

        void AddSharedVariableButtonStatusChange(string newText)
        {
            if (string.IsNullOrEmpty(newText))
            {
                addVariableButton.SetEnabled(false);
            }
            else
            {
                addVariableButton.SetEnabled(true);
            }
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
            selectTreeToolbar = root.Q<ToolbarMenu>("SelectTree");
            scrollView = root.Q<ScrollView>();
            deleteRunnerButton = root.Q<ToolbarButton>("DeleteRunner");
            addRunnerButton = root.Q<ToolbarButton>("AddRunner");
            var variableContainer = root.Q<IMGUIContainer>("AddVariableContainer");

            inputSharedValueName = variableContainer.Q<TextField>();
            addVariableButton = variableContainer.Q<Button>();

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
            addRunnerButton.clicked += AddRunnerComponent;
        }

        int selectedGameObjectInstanceID;


        void AddRunnerComponent()
        {
            var runner = Selection.activeGameObject;
            // this maybe change later, according to the requirement
            if (runner.GetComponent<BehaviourTreeRunner>())
            {
                return;
            }

            var behaviourTreeRunner = runner.AddComponent<BehaviourTreeRunner>();
            selectedGameObjectInstanceID = runner.GetInstanceID();

            // theGameObjectHasRunnerList.Add(runner);
            // AssignTreeDropDown();
            AddToolbarMenu(runner.name);

            AddIconToGameObject();
            InitBehaviourTree(behaviourTreeRunner);
            InitSharedVariableContainer(behaviourTreeRunner);
            // init sharedVariableContainer
            // UpdateCurrentSharedVariableView(behaviourTreeRunner.name);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnSelectionChange();
        }

        void AddIconToGameObject()
        {
            EditorApplication.hierarchyWindowItemOnGUI += AddIcon;
        }

        void DeleteIconFromGameObject()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= AddIcon;
        }

        void AddIcon(int instanceId, Rect selectionRect)
        {
            if (instanceId != selectedGameObjectInstanceID)
            {
                return;
            }

            Rect rect = new Rect(selectionRect);
            rect.x = rect.width + (selectionRect.x - 20f);
            rect.width = 20f;
            rect.height = 18f;
            GUI.Label(rect, icon);
        }

        void InitBehaviourTree(BehaviourTreeRunner behaviourTreeRunner)
        {
            if (behaviourTreeRunner.tree != null) return;

            behaviourTreeRunner.tree = CreateInstance<BehaviourTree>();
            if (Selection.activeObject is GameObject gameObject)
            {
                behaviourTreeRunner.treeName = $"{gameObject.name}";
                behaviourTreeRunner.tree.name = behaviourTreeRunner.treeName;
            }

            BehaviourTreeRunnerEditor.previousTreeName = behaviourTreeRunner.treeName;
            PlayerPrefs.SetString("previousTreeName", BehaviourTreeRunnerEditor.previousTreeName);
            string path = AssetResourceManager.GetBehaviourTreeAssetPath(behaviourTreeRunner.treeName);
            AssetDatabase.CreateAsset(behaviourTreeRunner.tree, path);
        }

        void InitSharedVariableContainer(BehaviourTreeRunner behaviourTreeRunner)
        {
            string assetPath = AssetResourceManager.GetSharedVariableContainerAssetPath(behaviourTreeRunner.treeName);
            AssetResourceManager.CreateScriptObjectAsset<SharedVariableContainer>(assetPath);
        }

        void DeleteBehaviourTreeRunnerComponent()
        {
            deleteRunnerButton.tooltip = "Delete Behaviour Tree Runner Component";
            deleteRunnerButton.clicked += () =>
            {
                if (Selection.activeGameObject == null)
                {
                    Debug.Log("Please select a GameObject before deleting Behaviour Tree Runner");
                    return;
                }

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

                    // theGameObjectHasRunnerList.Remove(runner);
                    // AssignTreeDropDown();
                    RemoveToolbarMenu(runner.name);

                    DeleteRelatedAssets(behaviourTreeRunner, treeName);
                    DeleteIconFromGameObject();
                }
            };
        }

        void DeleteRelatedAssets(BehaviourTreeRunner behaviourTreeRunner, string treeName)
        {
            string treeAssetPath = AssetResourceManager.GetBehaviourTreeAssetPath(treeName);
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
                selectTreeToolbar.text = Selection.activeGameObject.name;
                var runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if (runner)
                {
                    inspectorView.UpdateSelectedNodeInspector(null);
                    tree = runner.tree;
                    selectedTreeName = runner.treeName;
                    treeView.PopulateView(tree);
                    treeView.AutoFrameNode();

                    UpdateCurrentSharedVariableView(selectedTreeName);
                }
                else
                {
                    // todo: if tree is null, treeView should be reset, variable editor should be reset
                    tree = null;
                    treeView.PopulateView(null);
                    scrollView.Clear();
                    // createSharedVariableEditor.LoadSharedVariableContainerAsset(string.Empty);
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
                if (tree != null &&
                    AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    treeView.PopulateView(tree);
                }
            }
        }

        void SearchBehaviourTreeRunner()
        {
            var behaviourTreeRunners = FindObjectsOfType<BehaviourTreeRunner>();
            foreach (var behaviourTreeRunner in behaviourTreeRunners)
            {
                if (theGameObjectHasRunnerList.Contains(behaviourTreeRunner.gameObject))
                {
                    continue;
                }

                theGameObjectHasRunnerList.Add(behaviourTreeRunner.gameObject);
            }
        }

        void AssignTreeDropDown()
        {
            selectTreeToolbar.menu.MenuItems().Clear();
            var relativeGameObjects = theGameObjectHasRunnerList.Select(go => go.name).ToArray();
            Debug.Log(relativeGameObjects.Length);
            foreach (var relativeGameObject in relativeGameObjects)
            {
                selectTreeToolbar.menu.AppendAction(relativeGameObject, OnSelectTree);
            }
        }

        void AddToolbarMenu(string runnerName)
        {
            selectTreeToolbar.menu.AppendAction(runnerName, OnSelectTree);
        }

        void RemoveToolbarMenu(string runnerName)
        {
            var dropdownMenuItems = selectTreeToolbar.menu.MenuItems();
            for (int i = dropdownMenuItems.Count - 1; i >= 0; i--)
            {
                if (dropdownMenuItems[i] is DropdownMenuAction menuAction && menuAction.name == runnerName)
                {
                    selectTreeToolbar.menu.RemoveItemAt(i);
                    break; // 一旦找到并移除了项，就退出循环
                }
            }
        }


        void OnSelectTree(DropdownMenuAction action)
        {
            //todo: i don't know how to set the activeGameObject status to the selected one
            var selectedGameObject = theGameObjectHasRunnerList.FirstOrDefault(go => go.name == action.name);
            Selection.activeGameObject = selectedGameObject;

            OnSelectionChange();
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