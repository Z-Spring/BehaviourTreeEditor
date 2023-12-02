using MurphyEditor.BTree;

namespace BehaviourTreeEditor.BTree.CompositeNodes
{
    public class Selector : Composite
    {
        int currentChild;

        protected override void OnEnter()
        {
            currentChild = 0;
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            var child = children[currentChild];
            var state = child.Update();
            switch (state)
            {
                case State.Failure:
                    currentChild++;
                    if (currentChild >= children.Count)
                    {
                        currentChild = 0;
                        return State.Failure;
                    }

                    return State.Running;
                case State.Success:
                    return State.Success;
                default:
                    return State.Running;
            }
        }
    }
}