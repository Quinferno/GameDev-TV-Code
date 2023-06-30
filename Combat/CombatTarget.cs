using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
                if(!callingController.GetComponent<Fighter>().CanAttack(gameObject))//if can't attack target, continue to next item in array
                {
                    return false;
                }

                if(Input.GetMouseButton(0))
                {
                    callingController.GetComponent<Fighter>().Attack(gameObject);
                }
                return true;
        }
    }
}
