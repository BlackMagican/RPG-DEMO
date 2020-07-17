using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private bool isHoming = false;
        [SerializeField] private GameObject hitEffect = null;
        // These things will be destroyed immediately when trigger other things.
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] private float maxFlightTime = 2f;
        [SerializeField] private float lifeAfterImpact = 0.3f;
        
        private Health target = null;
        private float damage = 0f;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }
        
        void Update()
        {
            if (!target)
            {
                return;
            }

            // Some weapon may has chase function.
            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * 
                                (speed * Time.deltaTime));
        }

        public void SetTarget(Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
            Destroy(gameObject, maxFlightTime);
        }

        /// <summary>
        /// 
        ///  Get projectile's target position.
        /// 
        /// </summary>
        /// <returns>
        ///    Where should the projectile point. 
        /// </returns>
        private Vector3 GetAimLocation()
        {
            if (!target)
                return Vector3.forward;
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (!targetCapsule)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            /*
             * Don't let the projectile be stopped by other things
             * which has "Health" component too.
             *
             * And to prevent projectile be stopped by dead enemy's collider.
             */
            if (other.GetComponent<Health>() != target || target.IsDead())
                return;
            
            // Stop the projectile.
            speed = 0;
            
            if (hitEffect)
            {
                Instantiate(hitEffect, GetAimLocation(), 
                    transform.rotation);
            }
            target.TakeDamage(damage);

            /*
             * Let hit effect finish.
             */
            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
