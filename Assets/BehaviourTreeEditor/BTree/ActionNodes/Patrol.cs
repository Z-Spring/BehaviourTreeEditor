using BehaviourTreeEditor.SharedVariables;
using UnityEngine;

namespace BehaviourTreeEditor.BTree.ActionNodes
{
    public class Patrol : Action
    {
        public GameObject currentWaypoint;
        public SharedString waypointName;
        public SharedString sharedString;
        private int index;

        protected override void OnEnter()
        {
            currentGameContext.currentWaypoint = currentGameContext.waypoints[index];
            index++;
            if (index >= currentGameContext.waypoints.Count)
            {
                index = 0;
            }

            currentWaypoint = currentGameContext.currentWaypoint;
            waypointName.sharedValue = currentWaypoint.name;
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            currentGameContext.agent.SetDestination(currentWaypoint.transform.position);

            if (currentGameContext.agent.pathPending)
            {
                return State.Running;
            }

            if (currentGameContext.agent.remainingDistance <= currentGameContext.agent.stoppingDistance)
            {
                Debug.Log("Success");
                return State.Success;
            }

            if (currentGameContext.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            {
                Debug.Log("PathInvalid");
                return State.Failure;
            }


            return State.Running;
        }
    }
}