namespace BehaviourTreeEditor.BTree.CompositeNodes
{
    public class Sequence : Composite
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
            switch (child.Update())
            {
                case State.Running:
                    return State.Running;
                case State.Success:
                    currentChild++;
                    break;
                case State.Failure:
                    return State.Failure;
            }

            return currentChild == children.Count ? State.Success : State.Running;
        }
    }
}