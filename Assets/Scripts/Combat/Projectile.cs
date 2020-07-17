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

        private Health target = null;
        private float damage = 0f;

        // Update is called once per frame
        void Update()
        {
            if (!target)
            {
                return;
            }
            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * 
                                (speed * Time.deltaTime));
        }

        public void SetTarget(Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (!targetCapsule)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target)
                return;
            target.TakeDamage(damage);
            Destroy(gameObject, 0.2f);
        }
    }
}
