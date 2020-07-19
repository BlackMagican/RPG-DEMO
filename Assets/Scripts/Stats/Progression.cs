using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "progression", menuName = "RPG Project/Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] characterClasses = null;
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;
        
        
        public float GetStat(Stat stat, 
            CharacterClass characterClass, int level)
        {
            BuildLookup();
            float[] levels =  lookupTable[characterClass][stat];
            if (levels.Length < level)
                return 0;
            return levels[level - 1];
        }

        public float[] GetLevels(Stat stat,
            CharacterClass characterClass)
        {
            BuildLookup();
            float[] levels =  lookupTable[characterClass][stat];
            return levels;
        }

        private void BuildLookup()
        {
            if (lookupTable != null)
                return;
            
            lookupTable = new Dictionary<CharacterClass, 
                Dictionary<Stat, float[]>>();
            foreach (var progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach (var progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }
                
                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }
        
        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    
    }
}