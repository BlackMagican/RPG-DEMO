using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using System;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
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
            // This script works only works after having an attack target.
            if (target == null) return;
            if (!GetIsInRange())
            {
                mover.MoveTo(target.position);
            }else
            {
                mover.Cancel();
            }
        }

        /// <summary>
        ///
        /// Whether the target is within player's attack range.
        /// 
        /// 
        /// </summary>
        /// <returns>
        /// true: player should stop false: player should move
        /// </returns>
        private bool GetIsInRange()
        {
            return
                Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        /// <summary>
        /// This method is used to change actions from others to attack.
        /// </summary>
        /// <param name="combatTarget">Enemy who fight with player</param>
        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.transform;
        }

        /// <summary>
        /// Implement method from interface.
        /// 
        /// Cancel the attack action.
        /// </summary>
        public void Cancel()
        {
            target = null;
        }
    }
}
