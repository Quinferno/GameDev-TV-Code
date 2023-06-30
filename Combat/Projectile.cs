using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float projSpeed = 15;
        [SerializeField] bool homingProjectile = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifetime = 10;//Ten seconds should be plenty for most projectiles. It may even be too much.
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 0;
        Health projTarget = null;
        GameObject instigator = null; //Who shot the projectile (used for exp and maybe later crime/aggro system)
        float damage = 0;
        

        private void Start() 
        {
            transform.LookAt(GetAimLocation());
        }
        void Update()
        {
            if(projTarget == null) return;

            if(homingProjectile == true && !projTarget.IsDead())
            {
            transform.LookAt(GetAimLocation());
            }

            transform.Translate(Vector3.forward * projSpeed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.projTarget = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxLifetime); //destroys this projectile after max lifetime in secs has passed
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = projTarget.GetComponent<CapsuleCollider>();
            if(targetCapsule == null)
            {
                return projTarget.transform.position;
            }
            return projTarget.transform.position + Vector3.up * targetCapsule.height / 1.8f;
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(other.GetComponent<Health>() != projTarget) return;
            if(projTarget.IsDead()) return;
            projTarget.TakeDamage(instigator, damage);

            projSpeed = 0; //Prevents piercing as it is right now, may change later

            if(hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            if(destroyOnHit == null)
            {
                Destroy(gameObject);
                Destroy(hitEffect);
            }

            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
