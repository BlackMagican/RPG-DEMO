using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] float weaponRange = 2f;

        [SerializeField] Transform target = null;
        Mover mover;

        private void Start()
        {
            mover = GetComponent<Mover>();
        }

        private void Update()
        {
            if (target == null) return;
            if (!GetIsInRange())
            {
                // 如果不在攻击范围内则移动
                mover.MoveTo(target.position);
            }else
            {
                mover.Stop();
            }
        }

        private bool GetIsInRange()
        {
            return
                Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public void Attack(CombatTarget combatTarget)
        {
            target = combatTarget.transform;
        }

        public void Cancel()
        {
            target = null;
        }
    }
}
