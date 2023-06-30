using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using System;
using RPG.Core;
using RPG.Attributes;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float navMeshProjMax = 1f;
        [SerializeField] float interactSphereCastRadius = 1f;
        Health health;
        bool movementStarted = false;

        [System.Serializable] struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMapping = null;
        void Start()
        {
            health = GetComponent<Health>();
        }
        private void Update()
        {
            if(InteractWithUI()) return;

            if(health.IsDead()) 
            {
                SetCursor(CursorType.Dead);
                return;
            }

            if(InteractWithComponent()) return;

            if(InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), interactSphereCastRadius);
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();

                foreach (IRaycastable raycastable in raycastables)
                {
                    if(raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());//temp
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {   
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance; //gets distances of different hits
            }
            Array.Sort(distances, hits);//sorts hits by distances

            return hits;
        }
        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                movementStarted = false;
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMapping)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMapping[0];
        }

        private bool InteractWithMovement()
        {

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit == true)
            {
                if(GetComponent<Mover>().CanMoveTo(target) == false)
                {
                    return false;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    movementStarted = true;
                }
                if (Input.GetMouseButton(0) && movementStarted)
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);//1f is used to avoid reducing player speed while maintaining NPC patrol speed code
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);//Passes in ray and hit, and stores where raycast hit to give info elsewhere. Bool = true when there is a hit, which is useful.
            
            if(!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, navMeshProjMax, NavMesh.AllAreas);

            if(!hasCastToNavMesh) return false;
            
            target = navMeshHit.position;

            return true;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
