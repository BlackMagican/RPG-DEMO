using UnityEngine;

namespace Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] private GameObject targetToDestroy = null;
        void Update()
        {
            if (!GetComponent<ParticleSystem>().IsAlive())
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

