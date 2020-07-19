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
        private Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, 
                characterClass, startingLevel);
        }

        public int GetLevel()
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
    }
}