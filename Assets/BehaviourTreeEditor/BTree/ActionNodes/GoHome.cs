using UnityEngine;

namespace BehaviourTreeEditor.BTree.ActionNodes
{
    public class GoHome : Action
    {
        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            Debug.Log("GoHome");
            return State.Success;
        }
    }
}