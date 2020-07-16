using System;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;

namespace RPG.Movement
{
    /// <summary>
    /// This class will handel player's move behavior
    /// </summary>
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 6f;
        NavMeshAgent navMesh = null;
        Health health = null;

        void Awake()
        {
            navMesh = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            navMesh.enabled = !health.IsDead();
            UpdateAnimator();
        }

        [Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        /// <summary>
        ///
        /// This method is used to change actions from others to move.
        /// 
        /// </summary>
        /// <param name="destination">place to move</param>
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        /// <summary>
        ///
        /// Move the player to where they want to go.
        /// 
        /// </summary>
        /// <param name="speedFraction">
        /// Determine the speed when characters move.
        /// </param>
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMesh.destination = destination;
            navMesh.speed = maxSpeed * Mathf.Clamp01(speedFraction);
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

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData
            {
                position = new SerializableVector3(transform.position),
                rotation = new SerializableVector3(transform.eulerAngles)
            };
            return data;
        }

        public void RestoreState(object state)
        {
            if (state is MoverSaveData data)
            {
                GetComponent<NavMeshAgent>().enabled = false;
                transform.position = data.position.ToVector();
                transform.eulerAngles = data.rotation.ToVector();
                GetComponent<NavMeshAgent>().enabled = true;
            }
        }
    }
}
