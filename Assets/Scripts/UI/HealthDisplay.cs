using System;
using Resource;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health health;
        [SerializeField] private Text healthValue = null;
        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            if (!healthValue)
            {
                healthValue = GetComponent<Text>();
            }
        }

        private void Update()
        {
            healthValue.text = String.Format("{0:0} / {1:0}", health.Health1, health.FullHealth);
        }
    }
}
