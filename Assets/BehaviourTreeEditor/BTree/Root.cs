using UnityEngine;

namespace BehaviourTreeEditor.BTree
{
    public class Root : Node
    {
        [HideInInspector]public Node child;

        public override Node Clone()
        {
            Root node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            return child.Update();
        }
    }
}