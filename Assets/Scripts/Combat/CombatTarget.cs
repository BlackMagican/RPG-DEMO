using Control;
using Resource;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRayCastable
    {
        public bool HandleRayCast(PlayerController callingController)
        {
            var fighter = callingController.GetComponent<Fighter>();
            if (!fighter.CanAttack(gameObject))
            {
                return false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                fighter.Attack(gameObject);
            }
            
            return true;
        }
    }
}
