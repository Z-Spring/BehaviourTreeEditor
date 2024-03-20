using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class SetPatrolPoints : MonoBehaviour
    {
        public List<GameObject> waypoints;
        public GameObject currentWaypoint;

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Behavior Designer Hier Icon.png");
        }
    }
}