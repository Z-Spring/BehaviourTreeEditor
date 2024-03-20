using System;
using System.Collections.Generic;
using MurphyEditor.BTree.RunTime;
using UnityEditor;
using UnityEngine;

namespace BehaviourTreeEditor.BTree
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode;
        public Node.State treeState = Node.State.Running;
        public List<Node> nodes = new();

        public Node.State Update()
        {
            if (rootNode.state is Node.State.Running)
            {
                treeState = rootNode.Update();
            }

            return treeState;
        }

        public Node CreateNode(Type type)
        {
            Node node = CreateInstance(type) as Node;
            node.name = type.Name;
            node.nodeName = type.Name;
            node.guid = Guid.NewGuid().ToString();
            Undo.RecordObject(this, $"Behaviour Tree Add {node.name} node");
            nodes.Add(node);
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            Undo.RegisterCreatedObjectUndo(node, $"Behaviour Tree Add {node.name} node");
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, $"Behaviour Tree Remove {node.name} node");
            nodes.Remove(node);
            // AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
        }

        public void AddChild(Node parent, Node child)
        {
            if (parent is Composite compositeNode)
            {
                Undo.RecordObject(compositeNode, $"Behaviour Tree Add {compositeNode.name} Child");
                EditorUtility.SetDirty(compositeNode);
                compositeNode.children.Add(child);
            }
            else if (parent is Decorator decoratorNode)
            {
                Undo.RecordObject(decoratorNode, $"Behaviour Tree Add {decoratorNode.name} Child");
                EditorUtility.SetDirty(decoratorNode);
                decoratorNode.child = child;
            }
            else if (parent is Root rootNode)
            {
                Undo.RecordObject(rootNode, $"Behaviour Tree Add {rootNode.name} Child");
                EditorUtility.SetDirty(rootNode);
                rootNode.child = child;
            }
        }

        public void AddParent(Node child, Node parent)
        {
            if (child is Action actionNode)
            {
                actionNode.parent = parent;
            }
            else if (child is Conditional conditionNode)
            {
                conditionNode.parent = parent;
            }
            else if (child is Composite compositeNode)
            {
                compositeNode.parent = parent;
            }
            else if (child is Decorator decoratorNode)
            {
                decoratorNode.parent = parent;
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            if (parent is Composite compositeNode)
            {
                Undo.RecordObject(compositeNode, $"Behaviour Tree Remove {compositeNode.name} Child");
                EditorUtility.SetDirty(compositeNode);
                compositeNode.children.Remove(child);
            }
            else if (parent is Decorator decoratorNode)
            {
                Undo.RecordObject(decoratorNode, $"Behaviour Tree Remove {decoratorNode.name} Child");
                EditorUtility.SetDirty(decoratorNode);
                decoratorNode.child = null;
            }
            else if (parent is Root rootNode)
            {
                Undo.RecordObject(rootNode, $"Behaviour Tree Remove {rootNode.name} Child");
                EditorUtility.SetDirty(rootNode);
                rootNode.child = null;
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            if (parent is Composite compositeNode)
            {
                return compositeNode.children;
            }

            if (parent is Decorator decoratorNode)
            {
                if (decoratorNode.child != null)
                {
                    return new List<Node> { decoratorNode.child };
                }
            }

            if (parent is Root rootNode)
            {
                if (rootNode.child != null)
                {
                    return new List<Node> { rootNode.child };
                }
            }

            return new List<Node>();
        }

        // todo: can use this to traverse the tree
        void Traverse(Node node, Action<Node> visitor)
        {
            if (node)
            {
                visitor.Invoke(node);
                var children = GetChildren(node);
                // todo: traverse children
                children.ForEach(n => Traverse(n, visitor));
            }
        }

        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, n => { tree.nodes.Add(n); });
            tree.nodes.Add(tree.rootNode);
            return tree;
        }

        public void Bind(CurrentGameContext context)
        {
            Traverse(rootNode, node =>
            {
                node.currentGameContext = context;
            });
        }
    }
}