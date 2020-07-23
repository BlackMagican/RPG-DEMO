using System;
using GameDevTV.Utils;
using UnityEngine;

namespace Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;
        [SerializeField] private GameObject levelUpParticleEffect;
        [SerializeField] private bool shouldUseModifier;
        private Experience experience;

        public event Action OnLevelUp;

        private LazyValue<int> currentLevel;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            if (experience)
            {
                experience.onExpGained += UpdateLevel;
            }
            currentLevel.ForceInit();
        }

        /// <summary>
        ///
        /// When gain experience, call this method and calculate
        /// what level should player be now.
        /// 
        /// </summary>
        private void UpdateLevel()
        {
           int newLevel = CalculateLevel();
            if (newLevel> currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                if (OnLevelUp != null) 
                    OnLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        /// <summary>
        /// 
        /// Return different stat value.
        /// 
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public float GetStat(Stat stat)
        {
            float totalPoint = GetBaseStat(stat);
            if (shouldUseModifier)
            {
                totalPoint *= (1 +  GetPercentageModifier(stat) / 100);
                totalPoint += GetAdditiveModifiers(stat);
            }
            
            return totalPoint;
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, 
                characterClass, GetLevel());
        }

        /// <summary>
        /// Get the current level.
        /// </summary>
        /// <returns>
        ///     Character current level.
        /// </returns>
        private int GetLevel()
        {
            return currentLevel.value;
        }

        /// <summary>
        /// Calculate Level throw player's experience.
        /// </summary>
        /// <returns></returns>
        public int CalculateLevel()
        {
            if (!experience)
                return startingLevel;
            float currentExp = experience.ExperiencePoint;
            int secondLastLevel = progression.GetLevels(Stat.ExperienceToLevelUp,
                characterClass).Length;
            for (int level = 1; level <= secondLastLevel; level++)
            {
                // Get exp needed to level up.
                float expToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp,
                    characterClass, level);
                /*
                 * If current EXP > needed to level Up:
                 *     To check if we can upgrade 2 levels.
                 *     continue......
                 * Until:
                 * If current EXP < needed to level Up.
                 *     Return the final level.
                 */
                if (expToLevelUp > currentExp)
                    return level;
            }
            return secondLastLevel + 1;
        }

        /// <summary>
        /// 
        /// Get weapon's attribute bonus.(const value)
        /// 
        /// </summary>
        /// <param name="stat">
        /// Pass different stat, return the corresponding value.
        /// </param>
        /// <returns></returns>
        private float GetAdditiveModifiers(Stat stat)
        {
            if (!shouldUseModifier)
                return 0;
            float total = 0f;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifier(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
        
        /// <summary>
        /// 
        /// Get weapon's attribute bonus.(percentage value)
        /// 
        /// </summary>
        /// <param name="stat">
        /// Pass different stat, return the corresponding value.
        /// </param>
        /// <returns></returns>
        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifier)
                return 0;
            float totalBonus = 0f;
            foreach (var bonus in GetComponents<IModifierProvider>())
            {
                foreach (var enhance in bonus.GetPercentageModifier(stat))
                {
                    totalBonus += enhance;
                }
            }

            return totalBonus;
        }
    }
}