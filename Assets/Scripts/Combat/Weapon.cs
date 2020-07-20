using System;
using Resource;
using UnityEngine;

namespace Combat
{
    /// <summary>
    ///  Templates for all weapons.
    /// </summary>
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG Project/Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] float weaponDamage = 20f;
        [SerializeField] float weaponRange = 2f;
        // Player can't attack without interruption. 
        [SerializeField] private float timeBetweenAttack = 1f;
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] private Projectile projectile = null;
        [SerializeField] private bool isRightHanded = true;
        
        public GameObject hitEffect = null;

        private const string WeaponName = "Weapon";

        /// <summary>
        /// 
        /// Create a new weapon for player.
        /// 
        /// </summary>
        /// <param name="rightHand">
        ///
        /// Player's right hand position.
        /// 
        /// </param>
        /// <param name="leftHand">
        ///
        /// Player's left hand position.
        /// 
        /// </param>
        /// <param name="animator">
        ///
        /// Player's animator. 
        /// 
        /// </param>
        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            if (equippedPrefab)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                GameObject weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = WeaponName;
            }
            
            // Change the character's attack animation.
            var overrideController = 
                animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        /// <summary>
        ///
        /// When equip a new weapon, should destroy old weapon at first.
        /// 
        /// </summary>
        /// <param name="rightHand">
        /// Character's right hand position.
        /// </param>
        /// <param name="leftHand">
        /// Character's left hand position.
        /// </param>
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            // Find the old weapon at first.
            Transform oldWeapon = rightHand.Find(WeaponName);
            if (!oldWeapon)
            {
                oldWeapon = leftHand.Find(WeaponName);
            }
            if (!oldWeapon)
                return;
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        /// <summary>
        /// 
        /// Where the projectile should be launched.
        /// 
        /// </summary>
        /// <param name="rightHand">
        /// Character's right hand position.
        /// </param>
        /// <param name="leftHand">
        /// Character's left hand position.
        /// </param>
        /// <returns>
        ///     If this is a right-hand weapon,
        ///     it will return to the right hand position
        ///     otherwise return to the left hand position.
        /// </returns>
        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            handTransform = isRightHanded ? rightHand : leftHand;
            return handTransform;
        }

        /// <summary>
        ///
        /// To check if this weapon has a projectile.
        /// 
        /// </summary>
        /// <returns>
        ///     True: It has a projectile.
        ///     False: It doesn't have Projectile.
        /// </returns>
        public bool HasProjectile()
        {
            return projectile != null;
        }

        /// <summary>
        /// 
        /// When called this method, it will launch this weapon's
        /// Projectile.
        /// 
        /// </summary>
        /// <param name="rightHand">
        /// Character's right hand position.
        /// </param>
        /// <param name="leftHand">
        /// Character's left hand position.
        /// </param>
        /// <param name="target">
        /// Projectile point to.
        /// </param>
        /// <param name="instigator">
        /// Who launch an attack.
        /// </param>
        public void LaunchProjectile(Transform rightHand, 
            Transform leftHand, Health target, GameObject instigator, float calculateDamage)
        {
            Projectile projectileInstance =
                Instantiate(projectile, 
                    GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculateDamage);
        }
        
        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetTimeBetweenAttack()
        {
            return timeBetweenAttack;
        }
    }
}