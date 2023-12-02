using UnityEngine;

namespace BehaviourTreeEditor.BTree
{
    public abstract class Decorator : Node
    {
        // public Node parent;
        [HideInInspector]public Node child;

        public override Node Clone()
        {
            Decorator node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}