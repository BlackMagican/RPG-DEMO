using System;
using Combat;
using Resource;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter fighter = null;
        [SerializeField] private Text healthValue = null;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!fighter.Target)
            {
                healthValue.text = "N/A";
                return;
            }

            Health health = fighter.Target;
            //healthValue.text = String.Format("{0:0.0}%", health.GetPercentage());
            healthValue.text = String.Format("{0:0} / {1:0}", health.Health1, health.FullHealth);
        }
    }
}
