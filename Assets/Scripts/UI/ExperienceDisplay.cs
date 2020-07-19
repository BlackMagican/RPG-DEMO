using System;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ExperienceDisplay : MonoBehaviour
    {
        private Experience experience;
        [SerializeField] private Text expValue = null;
        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            if (!expValue)
            {
                expValue = GetComponent<Text>();
            }
        }

        private void Update()
        {
            expValue.text = String.Format("{0:0}", experience.ExperiencePoint);
        }
    }
}
