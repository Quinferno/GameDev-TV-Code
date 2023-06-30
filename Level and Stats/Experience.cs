using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour, ISaveable 
    {
        [SerializeField] float experiencePoints = 0;
        Health playerHealth;
        public event Action onExperienceGained;
        private void Awake()
        {
            playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
        }
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
            playerHealth.GetCurrentMaxHealth();
        }

        public float GetExperience()
        {
            return experiencePoints;
        }
        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}
