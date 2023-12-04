using BehaviourTreeEditor.SharedVariables;
using UnityEngine;

namespace BehaviourTreeEditor.BTree.ActionNodes
{
    public class SendEvent : Action
    {
        public SharedGameObject targetGameObject;
        public SharedString eventName;
        public SharedString sendMessage;
        BehaviourTreeRunner behaviourTreeRunner;

        protected override void OnEnter()
        {
            behaviourTreeRunner = targetGameObject.sharedValue.GetComponent<BehaviourTreeRunner>();
            if (behaviourTreeRunner == null)
            {
                Debug.LogError("Target GameObject does not have a BehaviourTreeRunner component");
            }
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            if (sendMessage == null)
            {
                behaviourTreeRunner.SendEvent(eventName.sharedValue);
            }
            else 
            {
                behaviourTreeRunner.SendEvent<object>(eventName.sharedValue, sendMessage.sharedValue);
            }
            
            return State.Success;
        }
    }
}