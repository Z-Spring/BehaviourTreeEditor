using System.Collections.Generic;
using DefaultNamespace;
// using com.spring;
using UnityEngine;
using UnityEngine.AI;

namespace MurphyEditor.BTree.RunTime
{
    public class CurrentGameContext
    {
        public GameObject gameObject;
        public Transform transform;
        public Rigidbody rb;
        public NavMeshAgent agent;
        public List<GameObject> waypoints;
        public GameObject currentWaypoint;

        public static CurrentGameContext CreateFromBehaviourTreeGameObject(GameObject gameObject)
        {
            return new CurrentGameContext
            {
                gameObject = gameObject,
                transform = gameObject.transform,
                // rb = gameObject.GetComponent<Rigidbody>(),
                agent = gameObject.GetComponent<NavMeshAgent>(),
                waypoints = gameObject.GetComponent<SetPatrolPoints>().waypoints,
                currentWaypoint = gameObject.GetComponent<SetPatrolPoints>().currentWaypoint
            };
        }
    }
}