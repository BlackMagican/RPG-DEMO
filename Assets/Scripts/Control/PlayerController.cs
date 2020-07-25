using Combat;
using Movement;
using Resource;
using UnityEngine;
using UnityEngine.EventSystems;

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
        [SerializeField] private CursorMapping[] cursors;
        Health health = null;
        private Mover mover = null;
        private Fighter fighter = null;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotSpot;
        }

        private void Awake()
        {
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
        }

        void Update()
        {
            if (InteractWithUI())
            {
                return;   
            }
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent())
                return;
            if (InteractWithMovement())
                return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (!EventSystem.current.IsPointerOverGameObject()) return false;
            SetCursor(CursorType.UI);
            return true;
        }
        
        private bool InteractWithComponent()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (var hit in hits)
            {
                IRayCastable[] detectables =
                    hit.transform.GetComponents<IRayCastable>();
                foreach (var detectable in detectables)
                {
                    if (!detectable.HandleRayCast(this)) 
                        continue;
                    SetCursor(CursorType.PickUp);
                    return true;
                }
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
        /// Returns whether player clicked on a movable location.
        /// </returns>
        private bool InteractWithMovement()
        {
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hit);
            if (!hasHit) return false;
            if (Input.GetMouseButton(1))
            {
                mover.StartMoveAction(hit.point, speedFraction);
            }
            SetCursor(CursorType.Movement);
            return true;
        }
        
        /// <summary>
        ///
        /// Set different for different behaviours.
        /// 
        /// </summary>
        /// <param name="type">
        /// Cursor's type.
        /// </param>
        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotSpot, CursorMode.Auto);
        }

        /// <summary>
        ///
        /// Get cursor's information from array.
        /// 
        /// </summary>
        /// <param name="type">
        /// Cursor's type.
        /// </param>
        /// <returns>
        ///    If find the correspond cursor, then return.
        ///    else return the first cursor.
        /// </returns>
        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (var cursor in cursors)
            {
                if (cursor.type == type)
                {
                    return cursor;
                }
            }

            return cursors[0];
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
