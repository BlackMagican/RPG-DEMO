﻿using System;
using System.Collections.Generic;
using Core;
using GameDevTV.Utils;
using Movement;
using Resource;
using Saving;
using Stats;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// This class implements combat behaviour.
    /// </summary>
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] Weapon defaultWeapon;
        [SerializeField] Transform rightHandTransform;
        [SerializeField] Transform leftHandTransform;
        [SerializeField] Health attackTarget;

        float timeSinceLastAttack = Mathf.Infinity;
        LazyValue<Weapon> currentWeapon;
        private BaseStats stats;
        private Animator animator;
        Mover mover;
        
        // 将 Get Component 放在 Awake 里可以避免出现捕获不到组件的问题
        private void Awake()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<Mover>();
            stats = GetComponent<BaseStats>();
            currentWeapon = new LazyValue<Weapon>(SetUpDefaultWeapon);
        }

        /// <summary>
        /// 
        /// Player will be spawned with default weapon.
        /// 
        /// </summary>
        /// <returns></returns>
        private Weapon SetUpDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            /* This script only works after having an attack attackTarget 
             * and this attackTarget is not dead.
             */
            if (!attackTarget || attackTarget.IsDead()) return;
            if (!GetIsInRange())
            {
                /* If character is too far from attackTarget,
                 * it should move to the attackTarget at first.
                 */
                mover.MoveTo(attackTarget.transform.position, 1f);
            }
            else
            {
                // Stop move and do attack behaviour.
                mover.Cancel();
                AttackBehaviour();
            }
        }

        /// <summary>
        /// 
        /// Equip player with new weapon. 
        /// 
        /// </summary>
        /// <param name="weapon">
        /// Weapon's type.
        /// </param>
        public void EquipWeapon(Weapon weapon)
        {
            if (!weapon) return;
            currentWeapon.value = weapon;
            AttachWeapon(weapon);

            if (Math.Abs(currentWeapon.value.HealthPercentageBonus) > 0)
            {
                print("reset health");
                gameObject.GetComponent<Health>().ReSetHealth();
            }

        }

        private void AttachWeapon(Weapon weapon)
        {
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        private void AttackBehaviour()
        {
            transform.rotation = Quaternion.LookRotation(
                attackTarget.transform.position - transform.position, 
                Vector3.up);
            if (timeSinceLastAttack > currentWeapon.value.GetTimeBetweenAttack())
            {
                // This will trigger Hit() event.
                TriggerAttack();
                /*
                    We have finished once attack,
                    so we should set this variable to 0.
                 */
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        /// <summary>
        ///
        /// Define whether characters can do attack behaviour.
        /// 
        /// </summary>
        /// <param name="combatTarget">Attack attackTarget</param>
        /// <returns>
        ///     True: characters can attack;
        ///     False: characters can't attack.
        /// </returns>
        public bool CanAttack(GameObject combatTarget)
        {
            if (!combatTarget)
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            // Health component can tell us whether this character is dead.
            return (targetToTest && !targetToTest.IsDead());
        }

        /// <summary>
        /// 
        /// Animation Events.
        /// It will be called automatically.
        ///
        /// ********************************************************
        ///     About Instigator                                   *
        ///  instigator is the character who launch the attack.    *
        ///  1. From Fighter.Hit() ----> Health.TakeDamage()       *
        ///                                                        *
        ///  If the combat attackTarget is killed                        *
        ///     Instigator will gain Experience from combat attackTarget.*
        ///                                                        *
        ///  2. From Fighter.Shoot() --> Weapon.LaunchProjectile() *
        ///  From LaunchProjectile() --> Projectile.SetTarget()    *
        ///  From Projectile --> Health.TakeDamage()               *
        /// ********************************************************
        /// 
        /// </summary>
        void Hit()
        {
            if (!attackTarget)
                return;
            
            if (currentWeapon.value)
            {
                if (!currentWeapon.value.HasProjectile() && 
                    currentWeapon.value.hitEffect)
                {
                    Vector3 location = GetEffectLocation();
                    Instantiate(currentWeapon.value.hitEffect, 
                        location, transform.rotation);
                }
                attackTarget.TakeDamage(gameObject, 
                   GetCalculatedDamage());
            }
            else
            {
                // If character doesn't have weapon,
                // it does damage by default value.
                attackTarget.TakeDamage(gameObject, GetCalculatedDamage());
            }
        }

        /// <summary>
        ///
        /// To define where the hit effect should be generated.
        /// 
        /// </summary>
        /// <returns>
        ///    Where are the effect generated.
        /// </returns>
        private Vector3 GetEffectLocation()
        {
            CapsuleCollider targetCollider = attackTarget.GetComponent<CapsuleCollider>();
            if (targetCollider)
            {
                return attackTarget.transform.position + 
                       Vector3.up * targetCollider.height / 2;
            }
            return attackTarget.transform.position;
        }

        /// <summary>
        /// 
        ///  Another animation event which has same function as hit.
        /// 
        /// </summary>
        void Shoot()
        {
            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, 
                    leftHandTransform, attackTarget, gameObject, GetCalculatedDamage());
            }
        }

        /// <summary>
        ///
        /// When character combat, this method will return
        /// character's basic damage + modifier damage.
        /// 
        /// </summary>
        /// <returns>
        /// Finally damage.
        /// </returns>
        private float GetCalculatedDamage()
        {
            float damage = stats.GetStat(Stat.Damage);
            return damage;
        }

        /// <summary>
        ///
        /// Whether the attackTarget is within player's attack range.
        /// 
        /// </summary>
        /// <returns>
        ///     True: player should stop
        ///     False: player should move
        /// </returns>
        private bool GetIsInRange()
        {
            return
                Vector3.Distance(transform.position, attackTarget.transform.position)
                < currentWeapon.value.GetWeaponRange();
        }

        /// <summary>
        /// 
        /// This method is used to change actions from others to attack.
        /// 
        /// </summary>
        /// <param name="combatTarget">
        /// 
        /// Enemy who fight with player
        /// 
        /// </param>
        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            attackTarget = combatTarget.GetComponent<Health>();
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
            attackTarget = null;
        }

        /// <summary>
        /// 
        /// cancel the attack behaviour.
        /// 
        /// </summary>
        private void StopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            switch (stat)
            {
                case Stat.Damage:
                    yield return currentWeapon.value.DamagePercentageBonus;
                    break;
                case Stat.Health:
                    yield return currentWeapon.value.HealthPercentageBonus;
                    break;
            }
        }

        public Health AttackTarget => attackTarget;

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            if (state is string weaponName)
            {
                Weapon weapon = Resources.Load<Weapon>(weaponName);
                EquipWeapon(weapon);
            }
        }
    }
}
