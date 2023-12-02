using BehaviourTreeEditor.SharedVariables;
using UnityEngine;

namespace BehaviourTreeEditor.BTree.Conditionals
{
    public class WithInsight : Conditional
    {
        public SharedGameObject target;
        public float fieldOfViewAngle = 90f;
        public float viewDistance = 100f;

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        protected override State OnUpdate()
        {
            // if (Vector3.Distance(currentGameContext.transform.position, target.SharedValue.transform.position) > viewDistance ||
            //     Vector3.Angle(currentGameContext.transform.forward,
            //         target.SharedValue.transform.position - currentGameContext.transform.position) > fieldOfViewAngle / 2f)
            // {
            //     Debug.Log("Out of sight");
            //     return State.Failure;
            // }
            if (target.sharedValue == null)
            {
                Debug.Log("Target is null");
                return State.Failure;
            }
            
            if (currentGameContext.transform == null)
            {
                Debug.Log("currentGameContext.transform is null");
                return State.Failure;
            }

            Debug.Log(Vector3.Angle(currentGameContext.transform.forward,
                target.sharedValue.transform.position - currentGameContext.transform.position));

            Debug.Log(Vector3.Distance(currentGameContext.transform.position, target.sharedValue.transform.position));
            return State.Success;
        }
    }
}