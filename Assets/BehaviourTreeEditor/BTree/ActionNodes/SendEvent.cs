using BehaviourTreeEditor.SharedVariables;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTreeEditor.BTree.ActionNodes
{
    public class SendEvent : Action
    {
        public SharedGameObject targetGameObject;
        public SharedString eventName;
        // send arguments
        public SharedVariable sendArg1;
        public SharedVariable sendArg2;
        
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

            if (sendArg1 == null)
            {
                behaviourTreeRunner.SendEvent(eventName.sharedValue);
            }
            else
            {
                if (sendArg2 == null)
                {
                    behaviourTreeRunner.SendEvent(eventName.sharedValue, sendArg1.GetValue());
                }
                else
                {
                    behaviourTreeRunner.SendEvent(eventName.sharedValue, sendArg1.GetValue(), sendArg2.GetValue());
                }
            }
           

            return State.Success;
        }
    }
}