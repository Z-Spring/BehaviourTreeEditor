using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeEditor.BTree
{
    public abstract class Composite : Node
    {
        // public Node parent;
        [HideInInspector]public List<Node> children = new List<Node>();
        
        public override Node Clone()
        {
            Composite node = Instantiate(this);
            
            node.children = children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}