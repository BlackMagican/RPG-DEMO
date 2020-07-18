using Combat;
using Movement;
using Resource;
using UnityEngine;

namespace Control
{
    /// <summary>
    ///
    /// This class controls all interations between player and environment
    /// 
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float speedFraction = 1f;
        Health health = null;
        private Mover mover = null;
        private Fighter fighter = null;

        private void Awake()
        {
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
        }

        void Update()
        {
            if (health.IsDead())
            {
                return;
            }
            if (InteractWithCombat())
                return;
            if (InteractWithMovement())
                return;
        }

        /// <summary>
        /// private bool InteractWithCombat -> Whether the player clicked enemy
        ///
        /// This will determine whether player clicked enemy. If the player
        /// clicked an enemy, it will call the "Attack" method.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// Returns whether player clicked an enemy
        /// </returns>
        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach(RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();

                if ((!target) ||
                    !fighter.CanAttack(target.gameObject))
                {
                    continue;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    fighter.Attack(target.gameObject);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///
        /// This will get the information from "Raycast" and let player move
        /// to the point where the ray first hit. 
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// Returns whether player clicked on a moveable location.
        /// </returns>
        private bool InteractWithMovement()
        {
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hit);
            if (!hasHit) return false;
            if (Input.GetMouseButton(1))
            {
                mover.StartMoveAction(hit.point, speedFraction);
            }
            return true;
        }

        /// <summary>
        ///
        /// Other methods use this to get the ray.
        /// 
        /// </summary>
        /// <returns> A ray from the camera </returns>
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
