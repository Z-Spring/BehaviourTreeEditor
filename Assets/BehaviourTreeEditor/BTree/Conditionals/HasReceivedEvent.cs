using BehaviourTreeEditor.BTree.SharedVariables;

namespace BehaviourTreeEditor.BTree.Conditionals
{
    public class HasReceivedEvent : Conditional
    {
        public SharedString eventName;
        public SharedVariable receivedArg1;
        public SharedVariable receivedArg2;

        BehaviourTreeRunner behaviourTreeRunner;
        bool hasRegistered;
        bool hasReceived;

        protected override void OnEnter()
        {
            behaviourTreeRunner = currentGameContext.gameObject.GetComponent<BehaviourTreeRunner>();
            if (!hasRegistered)
            {
                hasRegistered = true;
                behaviourTreeRunner.RegisterEvent(eventName.sharedValue, ReceivedEvent);
                behaviourTreeRunner.RegisterEvent<object>(eventName.sharedValue, ReceivedEvent);
                behaviourTreeRunner.RegisterEvent<object, object>(eventName.sharedValue, ReceivedEvent);
            }
        }

        protected override State OnUpdate()
        {
            return hasReceived ? State.Success : State.Failure;
        }

        protected override void OnExit()
        {
            if (hasReceived)
            {
                behaviourTreeRunner.UnRegisterEvent(eventName.sharedValue, ReceivedEvent);
                behaviourTreeRunner.UnRegisterEvent<object>(eventName.sharedValue, ReceivedEvent);
                behaviourTreeRunner.UnRegisterEvent<object, object>(eventName.sharedValue, ReceivedEvent);
                hasRegistered = false;
            }

            hasReceived = false;
        }


        private void ReceivedEvent()
        {
            hasReceived = true;
        }

        private void ReceivedEvent(object arg1)
        {
            ReceivedEvent();
            if (receivedArg1 != null)
            {
                receivedArg1.SetValue(arg1);
            }
        }

        private void ReceivedEvent(object arg1, object arg2)
        {
            ReceivedEvent();
            if (receivedArg1 != null)
            {
                receivedArg1.SetValue(arg1);
            }

            if (receivedArg2 != null)
            {
                receivedArg2.SetValue(arg2);
            }
        }
    }
}