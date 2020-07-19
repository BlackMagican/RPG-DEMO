using System;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelDisplay : MonoBehaviour
    {
        private BaseStats level = null;
        [SerializeField] private Text levelValue = null;
        private void Awake()
        {
            level = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            if (!levelValue)
            {
                levelValue = GetComponent<Text>();
            }
        }

        private void Update()
        {
            levelValue.text = String.Format("{0:0}", level.GetLevel());
        }
    }
}
