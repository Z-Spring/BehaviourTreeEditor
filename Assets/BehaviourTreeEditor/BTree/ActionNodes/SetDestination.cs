using UnityEngine;

namespace BehaviourTreeEditor.BTree.ActionNodes
{
    public class SetDestination : Action
    {
        int index;

        protected override void OnEnter()
        {
            currentGameContext.currentWaypoint = currentGameContext.waypoints[index];
            Debug.Log($"SetDestination: {currentGameContext.currentWaypoint.name}");
            index++;
            if (index >= currentGameContext.waypoints.Count)
            {
                index = 0;
            }
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}