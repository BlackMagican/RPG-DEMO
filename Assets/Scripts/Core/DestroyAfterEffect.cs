using System;
using UnityEngine;

namespace Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] private GameObject targetToDestroy = null;
        private ParticleSystem particle = null;

        private void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }

        void Update()
        {
            if (!particle.IsAlive())
            {
                if (targetToDestroy)
                {
                    Destroy(targetToDestroy);
                }
                else
                {
                    if (gameObject) 
                        Destroy(gameObject);   
                }
            }
        }
    }
}

