using System.Collections;
using Core;
using Movement;
using Resource;
using Saving;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// This class implements combat behaviour.
    /// </summary>
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float defaultDamage = 10f;
        [SerializeField] Weapon defaultWeapon = null;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Health target = null;

        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;
        private Animator animator = null;
        Mover mover;
        
        // 将 Get Component 放在 Awake 里可以避免出现捕获不到组件的问题
        private void Awake()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<Mover>();
        }

        private void Start()
        {
            if (!currentWeapon)
            {
                EquipWeapon(defaultWeapon);
            }
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

        public void TriggerAttack()
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
                target.TakeDamage(currentWeapon.GetWeaponDamage());
            }
            else
            {
                target.TakeDamage(defaultDamage);
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
                    leftHandTransform, target);
            }
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

        public object CaptureState()
        {
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            if (state is string weaponName)
            {
                Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
                EquipWeapon(weapon);
            }
        }
    }
}
