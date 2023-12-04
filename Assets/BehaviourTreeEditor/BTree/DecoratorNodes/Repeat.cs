namespace BehaviourTreeEditor.BTree.DecoratorNodes
{
    public class Repeat : Decorator
    {
        public bool restartOnSuccess = true;
        public bool restartOnFailure = true;

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            switch (child.Update())
            {
                case State.Running:
                    break;
                case State.Failure:
                    if (restartOnFailure)
                    {
                        return State.Running;
                    }

                    return State.Failure;
                case State.Success:
                    if (restartOnSuccess)
                    {
                        return State.Running;
                    }

                    return State.Success;
            }

            return State.Running;
        }
    }
}