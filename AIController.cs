using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving.Utils;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float quitDistance = 10f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] float waypointWaitTimer = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 0.5f;
        [SerializeField] float alertOthersDistance = 5f;
        [Range(0,1)] [SerializeField] float patrolSpeedFraction = 0.2f; //Multiplies with speed, so 0.2 means 20% max speed when patrolling, to a maximum of 100% at 1
        [SerializeField] public bool cannotAttack = false;
        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover; 
        LazyValue<Vector3> guardLocation;
        float timeSinceLastSawPlayer = Mathf.Infinity;//used for basic "suspicion system" after aggroing on player
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceAggravated = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player"); //Can probably also use this for interactable objects, using a different tag
            mover = GetComponent<Mover>();

            guardLocation = new LazyValue<Vector3>(GetGuardLocation);
        }

        private void Start()
        {
            guardLocation.ForceInit();
        }

        private Vector3 GetGuardLocation()
        {
            return transform.position;//Marks starting position so the AI can return to it when it loses aggression
        }
        private void Update()
        {
            if (health.IsDead()) return;

            if(cannotAttack) return;

            if (IsAggravated(player) && fighter.CanAttack(player))
            {
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            UpdateTimers();
        }
        public void Aggravate()
        {
            timeSinceAggravated = 0;
        }
        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        private void PatrolBehavior()
        {
            fighter.Cancel();
            Vector3 nextPosition = guardLocation.value;

            if(patrolPath != null)
            {
                if(AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            //Squiggly bracket used to be here

                if(timeSinceArrivedAtWaypoint > waypointWaitTimer)
                {
                    mover.StartMoveAction(nextPosition, patrolSpeedFraction);
                }
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;//tolerance is "wiggle room" on patrol path
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies()//Creates an aggression radius that causes an enemy to aggro surrounding enemies if aggro'd. May replace with or also add aggression groups later.
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, alertOthersDistance, Vector3.up, 0);

            foreach (RaycastHit hit in hits)
            {
                AIController AI = hit.collider.GetComponent<AIController>();

                if(AI == null) continue;

                else AI.Aggravate();
            }
        }

        private bool IsAggravated(GameObject player)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance || timeSinceAggravated < aggroCooldownTime;
        }
        private bool InQuitChaseRange(GameObject player)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < quitDistance || timeSinceAggravated < aggroCooldownTime;
        }

        //Called by Unity
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, chaseDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.transform.position, quitDistance);
        }
    }
}