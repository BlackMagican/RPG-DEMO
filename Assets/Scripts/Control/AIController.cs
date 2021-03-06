﻿using Combat;
using Core;
using GameDevTV.Utils;
using Movement;
using Resource;
using UnityEngine;

namespace Control
{
    /// <summary>
    /// This class implements enemy's behaviour.
    /// </summary>
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] GameObject player;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] private float wayPointTolerance = 1f;
        [SerializeField] float wayPointDwellTime = 2f;
        [Range(0, 1)] [SerializeField] float patrolSpeedFraction = 0.2f;

        Fighter fighter;
        Health health;
        Mover mover;
        LazyValue<Vector3> guardPosition;

        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWayPoint = Mathf.Infinity;
        int currentWayPointIndex;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        void Start()
        {
            guardPosition.ForceInit();
        }

        // Update is called once per frame
        void Update()
        {
            if (health.IsDead())
            {
                return;
            }
            /*
             * When player enters the warning distance,
             * it will attack player.
             */
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            /*
             * When player run out of chase distance,
             * it will wait for seconds to ensure that
             * his enemy has gone.
             */
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWayPoint += Time.deltaTime;
        }

        /// <summary>
        /// If enemy has patrol path,
        /// this method will let it keeping patrol along the patrol path.
        /// </summary>
        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null)
            {
                if (AtWayPoint())
                {
                    timeSinceArrivedAtWayPoint = 0;
                    CycleWayPoint();
                }
                nextPosition = GetCurrentWayPoint();
            }
            if (timeSinceArrivedAtWayPoint > wayPointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        /// <summary>
        /// Tell the character what waypoint is it standing.
        /// </summary>
        /// <returns>The waypoint's position.</returns>
        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWaypoint(currentWayPointIndex);
        }

        /// <summary>
        /// Whether character stands on the waypoint.
        /// </summary>
        /// <returns>
        ///     True: Character stands on the waypoint. 
        ///     False: Character is not stand on the waypoint.
        /// </returns>
        private bool AtWayPoint()
        {
            return Vector3.Distance(transform.position,
                GetCurrentWayPoint()) < wayPointTolerance;
        }

        /// <summary>
        /// Get next wayPoint's position.
        /// </summary>
        private void CycleWayPoint()
        {
            currentWayPointIndex = patrolPath.GetNextIndex(currentWayPointIndex);
        }

        /// <summary>
        /// Enemy's attack behaviour.
        /// </summary>
        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        /// <summary>
        /// Character will stand still and do nothing.
        /// </summary>
        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        /// <returns>
        ///     The distance between character and player.
        /// </returns>
        private bool InAttackRangeOfPlayer()
        {
            return Vector3.Distance(player.transform.position, transform.position)
                 < chaseDistance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}