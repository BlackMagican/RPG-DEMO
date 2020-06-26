using System;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    /// <summary>
    ///
    /// This class controls all interations between player and environment
    /// 
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        Health health;
        private void Start()
        {
            health = GetComponent<Health>();
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
        /// clicked an ememy, it will call the "Attack" method.
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

                if ((target == null) ||
                    !GetComponent<Fighter>().CanAttack(target.gameObject))
                {
                    continue;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    GetComponent<Fighter>().Attack(target.gameObject);
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
        /// Returns whetcher player clicked on a moveable location.
        /// </returns>
        private bool InteractWithMovement()
        {
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(1))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point);
                }
                return true;
            }
            return false;
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
