using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour//Goes on Patrol Path parent, not the individual waypoints
    {
        [SerializeField] bool showGizmos = false;
        [SerializeField] const float gizmoRadius = 0.5f;
        private void OnDrawGizmos()
        {
            if (showGizmos)
            {
                for (int i = 0; i < transform.childCount; i++) //loop goes from 0 to childCount -1, with children intended to be the waypoints. 
                {
                    int j = GetNextIndex(i);

                    Gizmos.DrawSphere(GetWaypoint(i), gizmoRadius);
                    Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
                }
            }
        }

        public int GetNextIndex(int i)
        {
            if(i +1 == transform.childCount)
            {
                return 0;//used to get j for last i in index, linking the first and last waypoint
            }
            return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
