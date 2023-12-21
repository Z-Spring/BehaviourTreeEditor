using System;
using BehaviourTreeEditor.BTree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public BehaviourTreeEditor.BTree.Node node;
        public NodePort InputNodePort;
        public NodePort OutputNodePort;
        public Action<NodeView> OnNodeSelected;

        public NodeView(BehaviourTreeEditor.BTree.Node node) : base(AssetResourceManager.NodeViewUXMLPath)
        {
            this.node = node;
            title = node.name;
            viewDataKey = node.guid;
            style.left = node.position.x;
            style.top = node.position.y;
            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
        }

        void SetupClasses()
        {
            if (node is BehaviourTreeEditor.BTree.Action)
            {
                AddToClassList("action");
            }
            else if (node is Composite)
            {
                AddToClassList("composite");
            }
            else if (node is Decorator)
            {
                AddToClassList("decorator");
            }
            else if (node is Conditional)
            {
                AddToClassList("condition");
            }
            // else if (node is BehaviourTreeEditor.BTree.SubTree)
            // {
            //     AddToClassList("subtree");
            // }
            else if (node is Root)
            {
                AddToClassList("root");
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour Tree Node Set Position");
            // node.position = newPos.position;
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            evt.menu.AppendAction("Edit Script", _ =>
            {
               MonoScript script = MonoScript.FromScriptableObject(node);
               AssetDatabase.OpenAsset(script);
            }, DropdownMenuAction.AlwaysEnabled);
            
            evt.menu.AppendSeparator();
        }

        void CreateInputPorts()
        {
            if (node is BehaviourTreeEditor.BTree.Action or Decorator or Conditional)
            {
                InputNodePort = new NodePort(Direction.Input, Port.Capacity.Single);
            }
            else if (node is Composite)
            {
                InputNodePort = new NodePort(Direction.Input, Port.Capacity.Multi);
            }

            if (InputNodePort == null)
            {
                return;
            }

            InputNodePort.portName = "";
            InputNodePort.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(InputNodePort);
            inputContainer[0].Q<NodePort>().portColor = Color.Lerp(Color.yellow, Color.green, 0.3f);
        }

        void CreateOutputPorts()
        {
            if (node is Decorator or Root)
            {
                OutputNodePort = new NodePort(Direction.Output, Port.Capacity.Single);
            }
            else if (node is Composite)
            {
                OutputNodePort = new NodePort(Direction.Output, Port.Capacity.Multi);
            }

            if (OutputNodePort == null)
            {
                return;
            }

            OutputNodePort.portName = "";
            OutputNodePort.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(OutputNodePort);
            outputContainer[0].Q<NodePort>().portColor = Color.Lerp(Color.red, Color.blue, 0.5f);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }

        public void SortChildren()
        {
            if (node.parent is Composite compositeNode)
            {
                compositeNode.children.Sort((a, b) => a.position.x.CompareTo(b.position.x));
            }
        }

        public void UpdateState()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("success");
            RemoveFromClassList("failure");
            if (!Application.isPlaying)
            {
                return;
            }

            switch (node.state)
            {
                case BehaviourTreeEditor.BTree.Node.State.Running:
                    if (node.started)
                    {
                        AddToClassList("running");
                    }

                    break;
                case BehaviourTreeEditor.BTree.Node.State.Success:
                    AddToClassList("success");
                    break;
                case BehaviourTreeEditor.BTree.Node.State.Failure:
                    AddToClassList("failure");
                    break;
            }
        }
    }
}