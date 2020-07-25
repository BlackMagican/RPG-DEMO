using System.Collections;
using Control;
using UnityEngine;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour, IRayCastable
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] private float respawnTime = 3f;

        /// <summary>
        /// Player can pick a new weapon.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PickUp(other.GetComponent<Fighter>());
            }
        }

        private void PickUp(Fighter fighter)
        {
            fighter.EquipWeapon(weapon);
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRayCast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
                PickUp(callingController.GetComponent<Fighter>());
            return true;
        }
    }
}
