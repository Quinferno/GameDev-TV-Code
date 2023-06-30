using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target = null;
        [SerializeField] public float maxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 40f;//max path length the player can ask the game to walk their character
        public bool clickOrderReceived = false;
        NavMeshAgent navMeshAgent;
        Health health;
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        void Update()
        {

            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {   
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);

        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if(!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false; 
            if(GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;

            if(path.corners.Length < 2) return total;

            for (int i =0; i < path.corners.Length-1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i+1]);
            }

            return total;
        }
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction); //Clamp makes sure number is always between 0 and 1
            navMeshAgent.isStopped = false;
            clickOrderReceived = true;
        }

        public void Stop()
        {
            navMeshAgent.isStopped = true;
            clickOrderReceived = false;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);//makes velocity local and helps tell animator what direction the player is moving and facing
            float speed = localVelocity.z;//all that matters for animations, right now, is z axis velocity (player's forward)
            GetComponent<Animator>().SetFloat("Forward Speed", speed);
        }

        public void Cancel()
        {
            Stop();
        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        { 
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            Dictionary<string, object> data = (Dictionary<string, object>)state;

            agent.enabled = false;
            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            agent.enabled = true;
        }
    }
}
