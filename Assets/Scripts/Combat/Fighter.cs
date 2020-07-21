using System.Collections.Generic;
using Core;
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
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider, IBuffProvider
    {
        [SerializeField] Weapon defaultWeapon;
        [SerializeField] Transform rightHandTransform;
        [SerializeField] Transform leftHandTransform;
        [SerializeField] Health target;
        [SerializeField] private BuffController buff;

        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon;
        private BaseStats stats;
        private Animator animator;
        Mover mover;
        
        // 将 Get Component 放在 Awake 里可以避免出现捕获不到组件的问题
        private void Awake()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<Mover>();
            stats = GetComponent<BaseStats>();
            buff = GetComponent<BuffController>();
        }

        private void Start()
        {
            if (!currentWeapon)
            {
                EquipWeapon(defaultWeapon);
            }
        }

        private void SetWeaponBuff()
        {
            if (!buff) return;
            float additive = currentWeapon.AdditiveBuffForPlayer;
            float percentage = currentWeapon.PercentageBuffForPlayer;
            buff.SetBuff(Stat.Damage, additive, percentage);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            /* This script only works after having an attack target 
             * and this target is not dead.
             */
            if (!target || target.IsDead()) return;
            if (!GetIsInRange())
            {
                /* If character is too far from target,
                 * it should move to the target at first.
                 */
                mover.MoveTo(target.transform.position, 1f);
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
            currentWeapon = weapon;
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
            SetWeaponBuff();
        }

        private void AttackBehaviour()
        {
            transform.rotation = Quaternion.LookRotation(
                target.transform.position - transform.position, 
                Vector3.up);
            if (timeSinceLastAttack > currentWeapon.GetTimeBetweenAttack())
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
        /// <param name="combatTarget">Attack target</param>
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
        ///  If the combat target is killed                        *
        ///     Instigator will gain Experience from combat target.*
        ///                                                        *
        ///  2. From Fighter.Shoot() --> Weapon.LaunchProjectile() *
        ///  From LaunchProjectile() --> Projectile.SetTarget()    *
        ///  From Projectile --> Health.TakeDamage()               *
        /// ********************************************************
        /// 
        /// </summary>
        void Hit()
        {
            if (!target)
                return;
            
            if (currentWeapon)
            {
                if (!currentWeapon.HasProjectile() && 
                    currentWeapon.hitEffect)
                {
                    Vector3 location = GetEffectLocation();
                    Instantiate(currentWeapon.hitEffect, 
                        location, transform.rotation);
                }
                target.TakeDamage(gameObject, 
                   GetCalculatedDamage());
            }
            else
            {
                // If character doesn't have weapon,
                // it does damage by default value.
                target.TakeDamage(gameObject, GetCalculatedDamage());
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
            CapsuleCollider targetCollider = target.GetComponent<CapsuleCollider>();
            if (targetCollider)
            {
                return target.transform.position + 
                       Vector3.up * targetCollider.height / 2;
            }
            return target.transform.position;
        }

        /// <summary>
        /// 
        ///  Another animation event which has same function as hit.
        /// 
        /// </summary>
        void Shoot()
        {
            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, 
                    leftHandTransform, target, gameObject, GetCalculatedDamage());
            }
        }

        private float GetCalculatedDamage()
        {
            float damage = stats.GetStat(Stat.Damage);
            return damage;
        }

        /// <summary>
        ///
        /// Whether the target is within player's attack range.
        /// 
        /// </summary>
        /// <returns>
        ///     True: player should stop
        ///     False: player should move
        /// </returns>
        private bool GetIsInRange()
        {
            return
                Vector3.Distance(transform.position, target.transform.position)
                < currentWeapon.GetWeaponRange();
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
                yield return currentWeapon.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.PercentageBonus;
            }
        }
        
        public IEnumerable<float> GetAdditiveBuff(Stat stat)
        {
            yield return buff.GetAdditiveBuff(stat);
        }

        public IEnumerable<float> GetPercentageBuff(Stat stat)
        {
            yield return buff.GetPercentageBuff(stat);
        }

        public Health Target => target;

        public object CaptureState()
        {
            return currentWeapon.name;
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
