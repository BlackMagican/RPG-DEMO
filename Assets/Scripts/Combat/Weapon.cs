using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG Project/Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] float weaponDamage = 20f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;

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
            if (equippedPrefab)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                Instantiate(equippedPrefab, handTransform);
            }
            if (animatorOverride)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            handTransform = isRightHanded ? rightHand : leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, 
            Transform leftHand, Health target)
        {
            Projectile projectileInstance =
                Instantiate(projectile, 
                    GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, weaponDamage);
        }
        
        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }
    }
}