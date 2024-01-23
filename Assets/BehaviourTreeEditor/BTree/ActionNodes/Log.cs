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
            Debug.Log($"{messagePrint.sharedValue}  ");
            return State.Success;
        }
    }
}