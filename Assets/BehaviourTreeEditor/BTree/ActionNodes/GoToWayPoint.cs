using BehaviourTreeEditor.SharedVariables;
using UnityEngine;

namespace BehaviourTreeEditor.BTree.ActionNodes
{
    public class GoToWayPoint : Action
    {
        public SharedGameObject target;

        // public SharedVector3 currentPosition ;
        public float speed = 8f;

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            currentGameContext.transform.position = Vector3.MoveTowards(currentGameContext.transform.position,
                target.sharedValue.transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(currentGameContext.transform.position, target.sharedValue.transform.position) < 1f)
            {
                return State.Success;
            }

            return State.Running;
        }
    }
}