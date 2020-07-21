using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    public class BuffController : MonoBehaviour
    {
        [SerializeField] private PlayerBuff buff;

        private void Awake()
        {
            if (!buff)
            {
                buff = GetComponent<PlayerBuff>();
            }
        }

        public void SetBuff(Stat stat, float additive, float percentage)
        {
            if (stat == Stat.Damage)
            {
                buff.AttackAdditiveBuff = additive;
                buff.AttackBuffPercentage = percentage;
            }else if (stat == Stat.Health)
            {
                buff.HealthAdditiveBuff = additive;
                buff.HealthBuffPercentage = percentage;
            }
        }

        public float GetAdditiveBuff(Stat stat)
        {
            if (stat == Stat.Health)
            {
                return buff.HealthAdditiveBuff;
            }else if (stat == Stat.Damage)
            {
                return buff.AttackAdditiveBuff;
            }

            return 0;
        }

        public float GetPercentageBuff(Stat stat)
        {
            if (stat == Stat.Health)
            {
                return buff.HealthBuffPercentage;
            }else if (stat == Stat.Damage)
            {
                return buff.AttackBuffPercentage;
            }

            return 0;
        }
    }
}
