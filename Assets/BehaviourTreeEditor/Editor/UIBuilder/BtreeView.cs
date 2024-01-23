using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTreeEditor.BTree;
using Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = UnityEditor.Experimental.GraphView.Node;

public class BtreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public static MyEdgeConnectorListener listener;
    public static event System.Action OnCreateTree;
    public bool HasUnsavedChanges { get; set; }
    BehaviourTree tree;
    Vector2 mousePosition;
    Button saveButton;
    string originalTitleText;

    EditorWindow editorWindow => EditorWindow.GetWindow<BTreeEditor>();

    public new class UxmlFactory : UxmlFactory<BtreeView, UxmlTraits>
    {
    }

    public BtreeView()
    {
        var gridBackground = new GridBackground();
        Insert(0, gridBackground);
        var styleSheet = AssetResourceManager.LoadAsset<StyleSheet>(AssetResourceManager.BTEditorUSSPath);
        styleSheets.Add(styleSheet);
        RegisterCallback();
        // AutoFrameNode();
        Undo.undoRedoPerformed += OnUndoRedo;
    }


    void OnUndoRedo()
    {
        Debug.Log("OnUndoRedo");
        PopulateView(tree);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void RegisterCallback()
    {
        this.AddManipulator(new ContentZoomer()
        {
            minScale = 0.1f,
            maxScale = 10f
        });
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new EdgeManipulator());
        RegisterCallback<KeyDownEvent>(OnKeyDownEvent);
        RegisterCallback<MouseDownEvent>(evt => mousePosition = evt.mousePosition);
    }

    public void SaveNodeAsset()
    {
        HasUnsavedChanges = false;
        UpdateTitleView();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void OnDataChanged()
    {
        //
        HasUnsavedChanges = true;
        UpdateTitleView();
    }

    private void UpdateTitleView()
    {
        if (HasUnsavedChanges && !editorWindow.titleContent.text.Contains("*"))
        {
            editorWindow.titleContent.text += "*";
        }
        else if (!HasUnsavedChanges && editorWindow.titleContent.text.Contains("*"))
        {
            editorWindow.titleContent.text = originalTitleText;
        }
    }


    void OnKeyDownEvent(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Space)
        {
            ShowAddNodeMenuFromSpaceKey();
        }

        if (evt.modifiers == EventModifiers.Control && evt.keyCode == KeyCode.D)
        {
            DuplicateNode();
        }
    }

    void ShowAddNodeMenuFromMouseClick()
    {
        Vector2 screenPosition = GUIUtility.GUIToScreenPoint(mousePosition);
        OpenSearchWindow(screenPosition);
    }

    private void ShowAddNodeMenuFromSpaceKey()
    {
        mousePosition = Event.current.mousePosition;
        Vector2 screenPosition = GUIUtility.GUIToScreenPoint(mousePosition);
        OpenSearchWindow(screenPosition);
    }

    public void OpenSearchWindow(Vector2 screenPosition)
    {
        var searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.Init(this);
        SearchWindow.Open(new SearchWindowContext(screenPosition), searchWindow);
    }

    void FindNodeView(BehaviourTreeEditor.BTree.Node node, out NodeView nodeView)
    {
        nodeView = GetNodeByGuid(node.guid) as NodeView;
    }

    public void PopulateView(BehaviourTree behaviourTree)
    {
        tree = behaviourTree;
        listener = new MyEdgeConnectorListener(this);

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
        if (tree == null)
        {
            return;
        }

        if (tree.rootNode == null)
        {
            CreateRootNode();
        }


        // // 方法组转换 创建节点
        tree.nodes.ForEach(InitNodeView);

        tree.nodes.ForEach(n =>
        {
            tree.GetChildren(n).ForEach(c =>
            {
                FindNodeView(n, out NodeView parentNodeView);
                FindNodeView(c, out NodeView childNodeView);
                AddElement(parentNodeView.OutputNodePort.ConnectTo(childNodeView.InputNodePort));
            });
        });

        editorWindow.rootVisualElement.Q<Label>("BehaviourTreeName").text = tree.name + "- Behaviour";
        editorWindow.titleContent.text = tree.name;
        originalTitleText = tree.name;
    }

    public void ResetView()
    {
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
    }

    void CreateRootNode()
    {
        tree.rootNode = tree.CreateNode(typeof(Root));
        NodeView nodeView = new NodeView(tree.rootNode);
        nodeView.SetPosition(new Rect(editorWindow.position.width / 2, editorWindow.position.height / 2, 100, 100));
        EditorUtility.SetDirty(tree);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    public void AutoFrameNode()
    {
        editorWindow.rootVisualElement.schedule.Execute(() =>
        {
            if (nodes.ToList().Count > 0)
            {
                FrameAll();
            }
            else
            {
                FrameOrigin();
            }
        }).StartingIn(50);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList()
            .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }


    // 删除节点
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
    {
        OnDataChanged();
        if (graphviewchange.elementsToRemove != null)
        {
            foreach (var element in graphviewchange.elementsToRemove)
            {
                if (element is NodeView nodeView)
                {
                    tree.DeleteNode(nodeView.node);
                }

                if (element is Edge edge && edge.input.node is NodeView inputNodeView &&
                    edge.output.node is NodeView outputNodeView)
                {
                    tree.RemoveChild(outputNodeView.node, inputNodeView.node);
                }
            }

            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
        }

        if (graphviewchange.edgesToCreate != null)
        {
            foreach (var edge in graphviewchange.edgesToCreate)
            {
                if (edge.input.node is NodeView inputNodeView && edge.output.node is NodeView outputNodeView)
                {
                    tree.AddChild(outputNodeView.node, inputNodeView.node);
                    tree.AddParent(inputNodeView.node, outputNodeView.node);
                }
            }
        }

        if (graphviewchange.movedElements != null)
        {
            foreach (var element in graphviewchange.movedElements)
            {
                if (element is NodeView nodeView)
                {
                    nodeView.SortChildren();
                }
            }
        }

        return graphviewchange;
    }

    private readonly List<string> scriptPaths = new List<string>()
    {
        "Assets/Editor/NodeTemplate/ActionNodeTemplate.txt",
        "Assets/Editor/NodeTemplate/CompositeNodeTemplate.txt",
        "Assets/Editor/NodeTemplate/DecoratorNodeTemplate.txt",
    };


    // 创建右键菜单选项
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (evt.target is Node)
        {
            evt.menu.AppendAction("Copy Node\tCtrl+D", a => DuplicateNode());
            return;
        }

        if (tree == null)
        {
            evt.menu.AppendAction("Create Tree", _ => OnCreateTree?.Invoke());
            return;
        }

        evt.menu.AppendAction("Add Node\tSpace", a => ShowAddNodeMenuFromMouseClick());
        evt.menu.AppendAction("Create Node/Action Node", a => CreateNodeTemplateScript(scriptPaths[0]));
        evt.menu.AppendAction("Create Node/Composite Node", a => CreateNodeTemplateScript(scriptPaths[1]));
        evt.menu.AppendAction("Create Node/Decorator Node", a => CreateNodeTemplateScript(scriptPaths[2]));
    }

    BehaviourTreeEditor.BTree.Node CloneNode(BehaviourTreeEditor.BTree.Node node)
    {
        var type = node.GetType();
        var newNode = tree.CreateNode(type);
        return newNode;
    }

    readonly List<ISelectable> newSelection = new List<ISelectable>();

    void DuplicateNode()
    {
        if (selection.Count >= 1)
        {
            selection.ForEach(s =>
            {
                if (s is NodeView nodeView)
                {
                    Vector2 currentPosition = nodeView.GetPosition().position;
                    BehaviourTreeEditor.BTree.Node node = CloneNode(nodeView.node);

                    CreateNodeViewForDuplicate(node, new Vector2(currentPosition.x + 100, currentPosition.y + 100));
                }
            });
        }
        else
        {
            return;
        }

        // todo: clearSelection 有问题   
        // selection.Clear();
        // ClearSelection();
        newSelection.ForEach(AddToSelection);
    }

    static void CreateNodeTemplateScript(string path)
    {
        // add script
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "NewNode.cs");
        // ProjectWindowUtil.CreateScriptAssetFromTemplateFile();
    }

    public void CreateNode(Type type, Vector2 position)
    {
        if (tree)
        {
            BehaviourTreeEditor.BTree.Node node = tree.CreateNode(type);
            if (node == null)
            {
                Debug.Log("node is null");
                return;
            }

            node.nodeName = type.Name;
            CreateNodeView(node, position);
        }
        else
        {
            Debug.LogError("BehaviourTree is null! You should create a new one first! ");
        }
    }

// 新创建的节点，放在屏幕的中央
    void CreateNodeView(BehaviourTreeEditor.BTree.Node node, Vector2 screenPosition)
    {
        Vector2 windowRectPosition = new Vector2(editorWindow.position.x, editorWindow.position.y);
        Vector2 localMousePosition = screenPosition - windowRectPosition;

        Vector2 graphMousePosition = contentViewContainer.WorldToLocal(localMousePosition);

        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        // this nodeSize is not the same as the nodeView's size, but very close
        // don't know how to get the nodeView's size
        Vector2 nodeSize = new Vector2(150, 76);
        // calculate the center position of the nodeView
        Vector2 adjustedPosition = graphMousePosition - nodeSize / 2;
        nodeView.SetPosition(new Rect(adjustedPosition, nodeSize));
        AddElement(nodeView);
        CreateNodeViewFromDropOutside(nodeView);
    }

    void CreateNodeViewFromDropOutside(NodeView lastNodeView)
    {
        if (listener.MyNodePort != null)
        {
            var outputNodePort = listener.MyNodePort;
            var edge = outputNodePort.ConnectTo(lastNodeView?.InputNodePort);
            AddElement(edge);
            var connectorListener = new DefaultEdgeConnectorListener();
            connectorListener.OnDrop(this, edge);
            // Listener.OnDrop(this, edge);
            listener.MyNodePort = null;
        }
    }


    void CreateNodeViewForDuplicate(BehaviourTreeEditor.BTree.Node node, Vector2 position)
    {
        // Vector2 graphMousePosition = contentViewContainer.WorldToLocal(position);

        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        Vector2 nodeSize = new Vector2(500, 500);
        nodeView.SetPosition(new Rect(position, nodeSize));
        AddElement(nodeView);
        newSelection.Add(nodeView);
    }

    void InitNodeView(BehaviourTreeEditor.BTree.Node node)
    {
        NodeView nodeView = new NodeView(node)
        {
            OnNodeSelected = OnNodeSelected
        };
        AddElement(nodeView);
    }


    public void UpdateNodeState()
    {
        nodes.ForEach(n =>
        {
            NodeView nodeView = n as NodeView;
            nodeView.UpdateState();
        });
    }
}