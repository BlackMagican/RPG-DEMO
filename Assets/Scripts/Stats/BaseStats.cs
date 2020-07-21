using System;
using UnityEngine;

namespace Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpParticleEffect = null;
        [SerializeField] private bool shouldUseModifier = false;
        [SerializeField] private bool canUseBuff = true;
        private Experience experience;

        public event Action OnLevelUp;

        private int currentLevel = 0;

        private void Awake()
        {
            experience = GetComponent<Experience>();
        }

        private void Start()
        {
            currentLevel = CalculateLevel();
            if (experience)
            {
                experience.onExpGained += UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                LevelUpEffect();
                if (OnLevelUp != null) 
                    OnLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            float totalPoint = GetBaseStat(stat);
            if (shouldUseModifier)
            {
                totalPoint +=  GetAdditiveModifiers(stat) * 
                       (1 +  GetPercentageModifier(stat) / 100);
            }

            if (canUseBuff)
            {
                float buffPoint = GetPercentageBuff(stat) / 100;
                totalPoint += totalPoint * buffPoint;
                totalPoint += GetAdditiveBuff(stat);
            }

            return totalPoint;

        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, 
                characterClass, GetLevel());
        }

        private int GetLevel()
        {
            if (currentLevel < 1)
                currentLevel = CalculateLevel();
            return currentLevel;
        }

        public bool CanUseBuff
        {
            set => canUseBuff = value;
        }

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

        private float GetPercentageBuff(Stat stat)
        {
            if (!canUseBuff)
                return 0;
            float total = 0f;
            foreach (var provider in GetComponents<IBuffProvider>())
            {
                foreach (float modifier in provider.GetPercentageBuff(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        private float GetAdditiveBuff(Stat stat)
        {
            if (!canUseBuff)
                return 0;
            float total = 0f;
            foreach (var provider in GetComponents<IBuffProvider>())
            {
                foreach (float modifier in provider.GetAdditiveBuff(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }
    }
}