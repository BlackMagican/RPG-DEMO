using Core;
using Saving;
using Stats;
using UnityEngine;

namespace Resource
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float health = 100f;
        private float fullHealth = 0f;

        bool isDead = false;

        private void Start()
        {
            this.health = GetComponent<BaseStats>().GetHealth();
            fullHealth = health;
        }

        public bool IsDead()
        {
            return isDead;
        }

        /// <summary>
        ///
        /// Decrease character's health.
        /// 
        /// </summary>
        /// <param name="damage">Reduced value</param>
        public void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0f);
            if (health <= 0)
            {
                Die();
            }
        }

        public float GetPercentage()
        {
            return 100 * health / fullHealth;
        }

        /// <summary>
        ///
        /// Character's behavior after death.
        /// 
        /// </summary>
        private void Die()
        {
            if (isDead)
            {
                return;
            }
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        
        public object CaptureState()
        {
            return this.health;
        }

        public void RestoreState(object state)
        {
            if (state is float health)
            {
                this.health = health;
            }

            if (this.health <= 0)
            {
                Die();
            }
        }
    }
}
