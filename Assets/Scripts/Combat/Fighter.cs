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
        // Player can't attack without interruption. 
        [SerializeField] float timeBetweenAttacks = 3f;
        [SerializeField] float weaponDamage = 10f;

        float timeSinceLastAttack = 0;

        [SerializeField] Health target = null;
        Mover mover;

        private void Start()
        {
            mover = GetComponent<Mover>();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            // This script works only works after having an attack target.
            if (target == null || target.IsDead()) return;
            if (!GetIsInRange())
            {
                mover.MoveTo(target.transform.position);
            }else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will trigger Hit() event.
                TriggerAttack();
                /*
                    we have finished once attack,
                    so we should set this variable to 0
                 */
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        public bool CanAttack(CombatTarget combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return (targetToTest != null) && !targetToTest.IsDead();
        }

        /// <summary>
        /// 
        /// Animation Events
        /// 
        /// </summary>
        void Hit()
        {
            if (target == null)
            {
                return;
            }
            target.TakeDamage(weaponDamage);
        }

        /// <summary>
        ///
        /// Whether the target is within player's attack range.
        /// 
        /// </summary>
        /// <returns>
        /// true: player should stop false: player should move
        /// </returns>
        private bool GetIsInRange()
        {
            return
                Vector3.Distance(transform.position, target.transform.position)
                < weaponRange;
        }

        /// <summary>
        /// 
        /// This method is used to change actions from others to attack.
        /// 
        /// </summary>
        /// <param name="combatTarget">Enemy who fight with player</param>
        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        /// <summary>
        /// Implement method from interface.
        /// 
        /// Cancel the attack action.
        /// 
        /// </summary>
        public void Cancel()
        {
            StopAttack();
            target = null;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }
    }
}
