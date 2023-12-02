using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTreeEditor.BTree;
using MurphyEditor.BTree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private BtreeView btreeView;
        Vector2 position;
        bool assemblyChanged;
       
        public void Init(BtreeView btreeView)
        {
            this.btreeView = btreeView;
        }

        #region Add SearchTreeEntry

        /*public List<SearchTreeEntry> CreateSearchTree2(SearchWindowContext context)
{
    var tree = new List<SearchTreeEntry>()
    {
        new SearchTreeGroupEntry(new GUIContent("Create Node")),
        new SearchTreeGroupEntry(new GUIContent("Root Node"), 1),
        new SearchTreeEntry(new GUIContent("   Root Node"))
        {
            level = 2,
            userData = typeof(RootNode)
        },
        new SearchTreeGroupEntry(new GUIContent("Action Nodes"), 1),
        new SearchTreeEntry(new GUIContent("   Print Node"))
        {
            level = 2,
            userData = typeof(PrintNode)
        },
        new SearchTreeGroupEntry(new GUIContent("Composite Node"), 1),
        new(new GUIContent("   Sequence  Node"))
        {
            level = 2,
            userData = typeof(SequenceNode)
        },
        new SearchTreeGroupEntry(new GUIContent("Decorator Node"), 1),

        new(new GUIContent("   Repeat Node"))
        {
            level = 2,
            userData = typeof(RepeatNode)
        }
    };
    return tree;
}*/


        #endregion

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            position = context.screenMousePosition;
            var tree = new List<SearchTreeEntry>();
          
            // get node types from all assemblies  
            // nodeTypes = AppDomain.CurrentDomain.GetAssemblies()
            //     .SelectMany(assembly => assembly.GetTypes())
            //     .Where(type => type.IsSubclassOf(typeof(Node))).ToList();

            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));

            var baseToChildTypes = BTreeEditor.nodeTypes
                .Where(t => !t.IsAbstract && t != typeof(Root))
                .ToLookup(t => t.BaseType);

            foreach (var baseType in baseToChildTypes)
            {
                if (baseType.Key.IsAbstract)
                {
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(baseType.Key.Name), 1));

                    foreach (var type in baseType)
                    {
                        tree.Add(new SearchTreeEntry(new GUIContent("   " + type.Name))
                        {
                            level = 2,
                            userData = type
                        });
                    }
                }
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry.userData is Type selectedType)
            {
                btreeView.CreateNode(selectedType, position);
                return true;
            }

            return false;
        }
    }
}