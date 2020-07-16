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

        /// <summary>
        /// 
        /// Create a new weapon for player.
        /// 
        /// </summary>
        /// <param name="handTransform">
        ///
        /// Player's hand position.
        /// 
        /// </param>
        /// <param name="animator">
        ///
        /// Player's animator. 
        /// 
        /// </param>
        public void Spawn(Transform handTransform, Animator animator)
        {
            if (equippedPrefab)
            {
                Instantiate(equippedPrefab, handTransform);
            }
            if (animatorOverride)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
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