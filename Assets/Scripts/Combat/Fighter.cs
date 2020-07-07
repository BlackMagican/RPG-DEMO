using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using System;
using RPG.Core;

namespace RPG.Combat
{
    /// <summary>
    /// This class implements combat behaviour.
    /// </summary>
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2f;
        // Player can't attack without interruption. 
        [SerializeField] float timeBetweenAttacks = 3f;
        [SerializeField] float weaponDamage = 10f;

        float timeSinceLastAttack = Mathf.Infinity;

        [SerializeField] Health target = null;
        Mover mover;

        private void Start()
        {
            mover = GetComponent<Mover>();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            // This script only works after having an attack target,
            // and this target is not dead.
            if (target == null || target.IsDead()) return;
            if (!GetIsInRange())
            {
                // If character is too far from target,
                // it should move to the target at first.
                mover.MoveTo(target.transform.position, 1f);
            }else
            {
                // stop move and do attack behaviour.
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

        public void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        /// <summary>
        ///
        /// Define whether characters can do attack behaviour.
        /// 
        /// </summary>
        /// <param name="combatTarget">Attack target</param>
        /// <returns>
        ///     True: characters can attack;
        ///     False: characters can't attack.
        /// </returns>
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            // Health component can tell us whether this character is dead.
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
        public void Attack(GameObject combatTarget)
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

        /// <summary>
        /// cancel the attack behaviour.
        /// </summary>
        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }
    }
}
