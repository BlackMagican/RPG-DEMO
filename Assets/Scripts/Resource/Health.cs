using Core;
using Saving;
using Stats;
using UnityEngine;

namespace Resource
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float reGenerationPercentage = 80;
        float health = -1f;
        private float fullHealth = 0f;

        bool isDead = false;

        private void Start()
        {
            GetComponent<BaseStats>().OnLevelUp += ReGenerateHealth;
            if (health < 0)
            {
                health = GetComponent<BaseStats>().GetStat(Stat.Health);
                fullHealth = health;
            }
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
        /// <param name="instigator">
        /// Who launch an attack.
        /// </param>
        /// <param name="damage">
        /// Reduced value
        /// </param>
        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + "took damage: " + damage);
            health = Mathf.Max(health - damage, 0f);
            if (health <= 0)
            {
                Die();
                AwardExperience(instigator);
            }
        }
        
        public float GetPercentage()
        {
            return 100 * health / fullHealth;
        }
        
        private void ReGenerateHealth()
        {
            float lastLevelHealthPoint = GetPercentage();
            fullHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
            float reSetPoint = Mathf.Max(lastLevelHealthPoint, 
                reGenerationPercentage);
            health = (reSetPoint * fullHealth) / 100;

        }

        public float FullHealth => fullHealth;

        public float Health1 => health;

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
            GetComponent<BaseStats>().CanUseBuff = false;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience exp = instigator.GetComponent<Experience>();
            if (!exp)
            {
                return;
            }

            exp.GainExperience(
                GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }
        


        public object CaptureState()
        {
            return this.health;
        }

        public void RestoreState(object state)
        {
            if (state is float healthPoint)
            {
                health = healthPoint;
            }

            if (health <= 0)
            {
                Die();
            }
        }
    }
}
