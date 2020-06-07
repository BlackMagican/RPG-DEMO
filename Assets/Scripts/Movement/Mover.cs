using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    /// <summary>
    /// This class will handel player's move behavior
    /// </summary>
    public class Mover : MonoBehaviour, IAction
    {
        NavMeshAgent navMesh = null;

        void Start()
        {
            navMesh = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            UpdateAnimator();
        }

        /// <summary>
        ///
        /// This method is used to change actions from others to move.
        /// 
        /// </summary>
        /// <param name="destination">place to move</param>
        public void StartMoveAction(Vector3 destination)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination);
        }

        /// <summary>
        ///
        /// Move the player to where they want to go.
        /// 
        /// </summary>
        /// <param name="destination"></param>
        public void MoveTo(Vector3 destination)
        {
            navMesh.destination = destination;
            navMesh.isStopped = false;
        }

        /// <summary>
        /// Let player stop.
        /// </summary>
        public void Cancel()
        {
            navMesh.isStopped = true;
        }

        /// <summary>
        /// Control the move animation.
        /// </summary>
        private void UpdateAnimator()
        {
            // Get player's current velocity.
            Vector3 velocity = navMesh.velocity;

            /*
             * Change the velocity to local velocity.
             * Then we can get the speed which the player runs forward.
             */
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("ForwardSpeed", speed);
        }
    }
}
