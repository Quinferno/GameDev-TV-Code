using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        void LateUpdate()//late update delays camera slightly to avoid jittering as player moves to play catch-up
        {
            transform.position = target.position;
        }
    }
}
