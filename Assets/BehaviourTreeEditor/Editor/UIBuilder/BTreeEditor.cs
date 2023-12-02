using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTreeEditor.BTree;
using BehaviourTreeEditor.BTree.ParentSharedVariable;
using BehaviourTreeEditor.SharedVariables;
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
        IMGUIContainer variableContainer;
        IMGUIContainer displayContainer;
        UnityEditor.Editor myeEditor;
        readonly List<UnityEditor.Editor> editorList = new();
        Dictionary<SharedVariable, IMGUIContainer> containers = new();
        public static VisualElement root; 
        static string titlename;
        

        // static int index = 1;
        public static List<Type> nodeTypes = new();

        // public static GUIContent TitleContent { get; set => titleContent = value; }
        ScrollView scrollView;

        static BehaviourTree tree;
        public static string selectedTreeName;

        readonly Dictionary<string, Type> typeMap = new()
        {
            { "string", typeof(SharedString) },
            { "int", typeof(SharedInt) },
            { "float", typeof(SharedFloat) },
            { "bool", typeof(SharedBool) },
            { "Vector2", typeof(SharedVector2) },
            { "Vector3", typeof(SharedVector3) },
            { "Quaternion", typeof(SharedQuaternion) },
            { "Color", typeof(SharedColor) },
            { "Rect", typeof(SharedRect) },
            { "GameObject", typeof(SharedGameObject) },
            { "Transform", typeof(SharedTransform) },
            { "Texture2D", typeof(SharedTexture2D) },
            { "Sprite", typeof(SharedSprite) },
            { "Material", typeof(SharedMaterial) },
            { "AnimationCurve", typeof(SharedAnimationCurve) },
            { "Object", typeof(SharedObject) },
            { "Collider", typeof(SharedCollider) },
            { "LayerMask", typeof(SharedLayerMask) }
        };

        BehaviourTreeRunnerEditor behaviourTreeRunnerEditor;


        [MenuItem("BTreeEditor/Show Window")]
        public static void OpenWindow()
        {
            string treeName = GetTreeName();
            titlename = treeName ?? $"BTreeEditor";
            GetWindow<BTreeEditor>(titlename);
            //
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

        SharedVariableContainer sharedVariableContainer;

        private void OnEnable()
        {
            GetNodeTypes();
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        SharedVariableContainer LoadSharedVariableContainer()
        {
            string treeName = GetTreeName();
            string assetPath = AssetResourceManager.GetSharedVariableContainerAssetPath(treeName);

            SharedVariableContainer sv = AssetResourceManager.LoadAsset<SharedVariableContainer>(assetPath);
            if (sv == null)
            {
                AssetResourceManager.CreateScriptObjectAsset<SharedVariableContainer>(assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return sv;
        }

        void InitializeVariableEditors()
        {
            string treeName = GetTreeName();
            editorList.Clear();
            scrollView.Clear();
            string assetPath = AssetResourceManager.GetSharedVariableContainerAssetPath(treeName);
            var variables = AssetResourceManager.LoadAllAssets<SharedVariable>(assetPath);

            foreach (var variable in variables)
            {
                CreateVariableEditor(variable);
            }
        }

        static string GetTreeName()
        {
            if (Selection.activeObject as GameObject == null)
            {
                return null;
            }

            GameObject gameObject = Selection.activeObject as GameObject;
            BehaviourTreeRunner behaviourTreeRunner = gameObject.GetComponent<BehaviourTreeRunner>();
            if (behaviourTreeRunner == null)
            {
                return null;
            }

            tree = behaviourTreeRunner.tree;
            if (tree == null)
            {
                Debug.Log("tree is null");
                return null;
            }

            return tree.name;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        void GetNodeTypes()
        {
            nodeTypes.Clear();
            nodeTypes = typeof(Node).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Node)))
                .ToList();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
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

        #region save changes before quitting

        private void OnDestroy()
        {
            if (treeView.UnsavedChanges)
            {
                treeView.SaveNodeAsset();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion

        public void CreateGUI()
        {
             root = rootVisualElement;
            // Each editor window contains a root VisualElement object
            // Instantiate UXML
            var visualTree = AssetResourceManager.LoadAsset<VisualTreeAsset>(AssetResourceManager.BTEditorUXMLPath);
            if (visualTree == null)
            {
                Debug.Log("visualTree is null");
            }

            visualTree.CloneTree(root);

            var styleSheet =
                AssetResourceManager.LoadAsset<StyleSheet>(AssetResourceManager.BTEditorUSSPath);
            root.styleSheets.Add(styleSheet);
            
            
            treeView = root.Q<BtreeView>();
            inspectorView = root.Q<InspectorView>();
            inspectorView = root.Q<InspectorView>();
            scrollView = root.Q<ScrollView>();
            variableContainer = root.Q<IMGUIContainer>("AddVariableContainer");
            displayContainer = root.Q<IMGUIContainer>("display-container");
            root.Q<Button>("SaveAsset").RegisterCallback<ClickEvent>(evt => treeView.SaveNodeAsset());


            HandleSelectTreeButton(root);
            CreateDropdownField(root);
            AddVariable(root);
            InitializeVariableEditors();
            AddBehaviourTreeRunnerComponent(root);
            DeleteBehaviourTreeRunnerComponent(root);
            treeView.OnNodeSelected += nodeView => inspectorView.PopulateView(nodeView);
            OnSelectionChange();
        }

        // todo: complete select tree menu
        void HandleSelectTreeButton(VisualElement root)
        {
            var toolbarMenu = root.Q<ToolbarMenu>("SelectTree");
            toolbarMenu.menu.AppendAction("Option 1", (a) => { Debug.Log("Option 1"); });
            toolbarMenu.menu.AppendAction("Option 2", (a) => { Debug.Log("Option 2"); });
            toolbarMenu.menu.AppendAction("Option 3", (a) => { Debug.Log("Option 3"); });
        }


        void AddVariable(VisualElement root)
        {
            var textField = variableContainer.Q<TextField>();

            var addVariableButton = variableContainer.Q<Button>();
            addVariableButton.clicked += () =>
            {
                string variableName = textField.text;
                string typeSelected = root.Q<DropdownField>().text;
                if (variableName == string.Empty)
                {
                    EditorUtility.DisplayDialog("Error", "Variable name cannot be empty.", "OK");
                    return;
                }

                CreateSharedVariableEditor(variableName, textField, typeSelected);
            };
        }

        void CreateSharedVariableEditor(string variableName, TextField textField, string typeSelected)
        {
            if (typeMap.TryGetValue(typeSelected, out Type type))
            {
                SharedVariable variable = CreateInstance(type) as SharedVariable;
                variable.name = variableName;
                AssetDatabase.AddObjectToAsset(variable, sharedVariableContainer);
                // PickAndMatchSharedVariableEditor.allSharedVariables.Add(variable.GetType());
                // PickAndMatchSharedVariableEditor.sharedScriptableObjectList.Add(variable);
                SharedVariablePopupEditor.AddSharedVariable(variable);
                CreateVariableEditor(variable);

                textField.SetValueWithoutNotify(string.Empty);
            }
            else
            {
                Debug.Log($"Type '{typeSelected}' not found in type map.");
            }
        }

        void CreateDropdownField(VisualElement root)
        {
            DropdownField field = root.Q<DropdownField>();
            field.choices.Clear();
            field.choices.AddRange(typeMap.Keys);
            string firstValue = field.choices.FirstOrDefault();
            field.SetValueWithoutNotify(firstValue);
        }

        void CreateVariableEditor(SharedVariable variable)
        {
            IMGUIContainer newContainer = new IMGUIContainer();
            newContainer.style.marginBottom = 10;
            newContainer.style.marginRight = 15;
            if (variable == null)
            {
                return;
            }

            containers[variable] = newContainer;

            int editorIndex = editorList.FindIndex(editor => editor.target == variable);
            UnityEditor.Editor currentEditor = null;
            if (editorIndex < 0)
            {
                currentEditor = UnityEditor.Editor.CreateEditor(variable);
                editorList.Add(currentEditor);
            }
            else
            {
                UnityEditor.Editor.CreateCachedEditor(variable, null, ref currentEditor);
                editorList[editorIndex] = currentEditor;
            }

            newContainer.onGUIHandler = () =>
            {
                if (currentEditor.target != null)
                {
                    currentEditor.OnInspectorGUI();
                    GUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Delete Variable", GUILayout.ExpandWidth(true)))
                    {
                        //调用删除函数
                        DeleteVariable(variable, sharedVariableContainer, editorList, currentEditor);
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();

                    Rect separatorRect =
                        EditorGUILayout.GetControlRect(false, 1.5f); // 1 is the height of line
                    // EditorStyles.label.normal.background = Texture2D.whiteTexture;
                    GUI.Box(separatorRect, GUIContent.none);
                }
            };
            scrollView.Add(newContainer);
        }

        private void DeleteVariable(SharedVariable variable, SharedVariableContainer sv,
            List<UnityEditor.Editor> el, UnityEditor.Editor currentEditor)
        {
            AssetDatabase.RemoveObjectFromAsset(variable);
            el.Remove(currentEditor);
            DestroyImmediate(currentEditor);
            if (containers.TryGetValue(variable, out IMGUIContainer container))
            {
                scrollView.Remove(container);
                // container.RemoveFromHierarchy();
                containers.Remove(variable);
                // PickAndMatchSharedVariableEditor.RemoveSharedVariable(variable);
                SharedVariablePopupEditor.RemoveSharedVariable(variable);
            }
        }

        void AddBehaviourTreeRunnerComponent(VisualElement root)
        {
            var addRunnerButton = root.Q<ToolbarButton>("AddRunner");
            addRunnerButton.tooltip = "Add Behaviour Tree Runner Component";
            addRunnerButton.clicked += () =>
            {
                var runner = Selection.activeGameObject;
                if (runner.GetComponent<BehaviourTreeRunner>())
                {
                    return;
                }

                BehaviourTreeRunner behaviourTreeRunner = runner.AddComponent<BehaviourTreeRunner>();

                CreateAndSaveNewBehaviorTree(behaviourTreeRunner);
                root.Query<Label>("BehaviourTreeName").First().text =
                    behaviourTreeRunner.treeName + "- Behaviour";
                OnSelectionChange();
                // behaviourTreeRunner.tree = Selection.activeObject as BehaviourTree;
            };
        }

        void DeleteBehaviourTreeRunnerComponent(VisualElement root)
        {
            var deleteRunnerButton = root.Q<ToolbarButton>("DeleteRunner");
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

                    treeView.ResetView();
                    root.Query<Label>("BehaviourTreeName").First().text = "Add A Behaviour Tree Runner";
                    titleContent.text = "BTreeEditor";
                    // treeView.PopulateView(behaviourTreeRunner.tree);
                    string assetPath = AssetDatabase.GetAssetPath(behaviourTreeRunner.tree);
                    string treeName = GetTreeName();
                    AssetDatabase.DeleteAsset(
                        $"Assets/BehaviourTreeEditor/Editor/{treeName}SharedVariableContainer.asset");
                    AssetDatabase.DeleteAsset(assetPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    DestroyImmediate(behaviourTreeRunner);
                }
            };
        }

        void CreateAndSaveNewBehaviorTree(BehaviourTreeRunner behaviourTreeRunner)
        {
            if (behaviourTreeRunner.tree == null)
            {
                behaviourTreeRunner.tree = CreateInstance<BehaviourTree>();
                if (behaviourTreeRunner.treeName != null)
                {
                    behaviourTreeRunner.tree.name = behaviourTreeRunner.treeName;
                }
                else
                {
                    if (Selection.activeObject is GameObject gameObject)
                    {
                        behaviourTreeRunner.treeName = $"{gameObject.name}";
                        behaviourTreeRunner.tree.name = behaviourTreeRunner.treeName;
                    }
                }

                BehaviourTreeRunnerEditor.previousTreeName = behaviourTreeRunner.treeName;
                PlayerPrefs.SetString("previousTreeName", BehaviourTreeRunnerEditor.previousTreeName);
                AssetDatabase.CreateAsset(behaviourTreeRunner.tree, $"Assets/{behaviourTreeRunner.treeName}.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void OnSelectionChange()
        {

            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (!tree)
            {
                if (Selection.activeGameObject)
                {
                    var runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                    if (runner)
                    {
                        treeView.AutoFrameNode();
                        selectedTreeName = runner.treeName;
                        sharedVariableContainer = LoadSharedVariableContainer();
                        InitializeVariableEditors();
                        // PickAndMatchSharedVariableEditor.UpdateSharedVariables();
                        inspectorView.PopulateView(null);
                        tree = runner.tree;
                    }
                }
            }

            if (Application.isPlaying)
            {
                if (tree)
                {
                    treeView.PopulateView(tree);
                }
            }
            else
            {
                if (tree != null && /*todo: dont understand*/
                    AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    treeView.PopulateView(tree);
                }
            }

        }

        // todo: update node state
        // frequency: 10 times per second
        void OnInspectorUpdate()
        {
            treeView?.UpdateNodeState();
        }
    }
}