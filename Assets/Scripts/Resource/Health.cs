using System;
using Core;
using GameDevTV.Utils;
using Saving;
using Stats;
using UnityEngine;

namespace Resource
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float reGenerationPercentage = 80;
        LazyValue<float> health;
        float fullHealth;

        bool isDead = false;

        private void Awake()
        {
            health = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            GetComponent<BaseStats>().OnLevelUp += ReGenerateHealth;
            health.ForceInit();
            fullHealth = health.value;
        }

        public bool IsDead()
        {
            return isDead;
        }
        
        /// <summary>
        /// 
        /// Reset character's health when equip a weapon.
        /// 
        /// </summary>
        public void ReSetHealth()
        {
            float temp = fullHealth;
            fullHealth = GetInitialHealth();
            if (Math.Abs(health.value - temp) < 0.2f)
            {
                health.value = fullHealth;
            }

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
            health.value = Mathf.Max(health.value - damage, 0f);
            if (health.value <= 0)
            {
                Die();
                AwardExperience(instigator);
            }
        }
        
        /// <summary>
        /// Get character's health percentage for display.
        /// </summary>
        /// <returns></returns>
        private float GetPercentage()
        {
            return 100 * health.value / fullHealth;
        }
        
        /// <summary>
        /// 
        /// When player level up, reset the health.
        /// 
        /// </summary>
        private void ReGenerateHealth()
        {
            float lastLevelHealthPoint = GetPercentage();
            fullHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
            float reSetPoint = Mathf.Max(lastLevelHealthPoint, 
                reGenerationPercentage);
            health.value = (reSetPoint * fullHealth) / 100;

        }

        public float FullHealth => fullHealth;

        public float Health1 => health.value;

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

        /// <summary>
        /// 
        /// This method will be called When player killed an enemy
        /// and gain experience.
        /// 
        /// </summary>
        /// <param name="instigator">
        /// Who will gain the experience.
        /// </param>
        private void AwardExperience(GameObject instigator)
        {
            if (!instigator.CompareTag("Player"))
            {
                return;
            }
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
            if (state is LazyValue<float> healthPoint)
            {
                health = healthPoint;
            }

            if (health.value <= 0)
            {
                Die();
            }
        }
    }
}
