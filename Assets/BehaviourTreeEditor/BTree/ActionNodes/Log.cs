using BehaviourTreeEditor.BTree.SharedVariables;
using UnityEngine;

namespace BehaviourTreeEditor.BTree.ActionNodes
{
    public class Log : Action
    {
        public SharedString messagePrint;
        public SharedInt number;

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            if (messagePrint is null)
            {
                Debug.LogError("Message is null");
                return State.Failure;
            }

            Debug.Log($"{messagePrint.sharedValue}  ");
            return State.Success;
        }
    }
}