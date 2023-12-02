using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTreeEditor.BTree;
using Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BtreeView : GraphView
{
    BehaviourTree tree;
    Vector2 mousePosition;
    readonly Dictionary<string, Vector2> nodePositions = new();
    public Action<NodeView> OnNodeSelected;
    public static MyEdgeConnectorListener Listener;
    private Button saveButton;
    public bool UnsavedChanges { get; set; }

    private string originalTitleText;

    BTreeEditor editorWindow = ScriptableObject.CreateInstance<BTreeEditor>();
    // EditorWindow editorWindow ;
    // EditorWindow editorWindow2 => EditorWindow.GetWindow<BTreeEditor>();

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

        Undo.undoRedoPerformed += OnUndoRedo;
    }


    void OnUndoRedo()
    {
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
        UnsavedChanges = false;
        UpdateTitleView();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void OnDataChanged()
    {
        //
        UnsavedChanges = true;
        UpdateTitleView();
    }

    private void UpdateTitleView()
    {
        if (UnsavedChanges && !editorWindow.titleContent.text.Contains("*"))
        {
            editorWindow.titleContent.text += "*";
        }
        else if (!UnsavedChanges && editorWindow.titleContent.text.Contains("*"))
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
            Debug.Log("DuplicateNode Success!");
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
        Listener = new MyEdgeConnectorListener(this);

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
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

        BTreeEditor.root.Q<Label>("BehaviourTreeName").text = tree.name + "- Behaviour";
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
        BTreeEditor.root.schedule.Execute(() =>
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
        evt.menu.AppendAction("CopyNode\tCtrl+D", a => DuplicateNode());
        evt.menu.AppendAction("SelectNode\tSpace", a => ShowAddNodeMenuFromMouseClick());
        evt.menu.AppendAction("CreateNode/Action Node", a => CreateNodeTemplateScript(scriptPaths[0]));
        evt.menu.AppendAction("CreateNode/Composite Node", a => CreateNodeTemplateScript(scriptPaths[1]));
        evt.menu.AppendAction("CreateNode/Decorator Node", a => CreateNodeTemplateScript(scriptPaths[2]));
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
            // ClearSelection();
            // int index = 1;
            selection.ForEach(s =>
            {
                if (s is NodeView nodeView)
                {
                    Vector2 currentPosition = nodeView.GetPosition().position;
                    BehaviourTreeEditor.BTree.Node node = CloneNode(nodeView.node);

                    // var offset = new Vector2(index * 100, 0);
                    CreateNodeViewForDuplicate(node, new Vector2(currentPosition.x + 100, currentPosition.y + 100));

                    // index++;
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
            Debug.LogError("BehaviourTree is null! You can select a tree in the project window or create a new one. ");
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
        Vector2 nodeSize = new Vector2(500, 500);
        nodeView.SetPosition(new Rect(graphMousePosition, nodeSize));
        AddElement(nodeView);
        CreateNodeViewFromDropOutside(nodeView);
    }

    void CreateNodeViewFromDropOutside(NodeView lastNodeView)
    {
        if (Listener.MyNodePort != null)
        {
            var outputNodePort = Listener.MyNodePort;
            var edge = outputNodePort.ConnectTo(lastNodeView?.InputNodePort);
            AddElement(edge);
            var connectorListener = new DefaultEdgeConnectorListener();
            connectorListener.OnDrop(this, edge);
            // Listener.OnDrop(this, edge);
            Listener.MyNodePort = null;
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