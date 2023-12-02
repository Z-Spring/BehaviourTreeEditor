using BehaviourTreeEditor.SharedVariables;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTreeEditor.BTree.ActionNodes
{
    public class Log : Action
    {
        [FormerlySerializedAs("message")] public SharedString messagePrint;
        public SharedInt number;
        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            Debug.Log($"{messagePrint.sharedValue }  ");
            return State.Success;
        }
    }
}