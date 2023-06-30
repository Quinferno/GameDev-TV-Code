using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject targetToDestroy = null;

        void Update() 
        {
            if(!GetComponent<ParticleSystem>() == false && !GetComponent<ParticleSystem>().IsAlive())
            {
                if(targetToDestroy != null)
                {
                    Destroy(targetToDestroy);
                }

                else
                
                foreach(Transform child in this.transform)
                {
                    Destroy(child.gameObject);
                }
                Destroy(gameObject);
            }
        }
    }
}