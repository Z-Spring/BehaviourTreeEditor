using BehaviourTreeEditor.SharedVariables;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTreeEditor.BTree.Conditionals
{
    public class HasReceivedEvent : Conditional
    {
        public SharedString eventName;
        public SharedString receivedMessage;

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
            if (receivedMessage != null)
            {
                receivedMessage.sharedValue = (string)arg1;
            }
        }
    }
}