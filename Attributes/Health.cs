using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using RPG.Core;
using RPG.Saving.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] UnityEvent<float> takeDamage;
        public LazyValue<float> maxHealth;
        public LazyValue<float> health;
        bool isDead = false;
        bool isHurt = false;
        bool hasAwardedExperience = false;

        private void Awake()
        {
            health = new LazyValue<float>(GetInitialHealth);
            maxHealth = new LazyValue<float>(GetCurrentMaxHealth);
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }
        private void Start()
        {
            health.ForceInit();
            maxHealth.ForceInit();
        }
        private void Update()
        {
            maxHealth.value = GetComponent<BaseStats>().GetStat(Stats.Health);
        }
        public bool IsDead()
        {
            return isDead;
        }
        public bool IsHurt()
        {
            return isHurt;
        }

        public void Heal(float heal)
        {
            if(health.value >= 0)
            {
            health.value = Mathf.Min(health.value + heal, maxHealth.value);
            }
        }
        public void TakeDamage(GameObject instigator, float damage)
        {
            health.value = Mathf.Max(health.value - damage, 0);           
            {
                takeDamage.Invoke(damage);//Can place in an else statement after if statement to make text not appear on death
                if(health.value <= 0)
                {
                    Die();
                    AwardExperience(instigator);
                }
            }
            
            isHurt = true;
        }
        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stats.Health);
        }
        public float GetCurrentMaxHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stats.Health);
        }
        public float GetCurrentHealth()
        {
            return health.value;
        }
        public float GetMaxHealth()
        {
            return maxHealth.value;
        }
        private void RegenerateHealth()
        {
            maxHealth.value = GetComponent<BaseStats>().GetStat(Stats.Health);
            health.value = Mathf.Max(maxHealth.value * .8f, health.value);
        }
        private void Die()
        {
            if(isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if(experience == null) return;
            
            if(hasAwardedExperience == false)
            {
            hasAwardedExperience = true;
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stats.ExperienceReward));
            }
        }

        public object CaptureState()
        {
            return health.value;
        }

        public void RestoreState(object state)
        {
            health.value = (float)state;

            if(health.value <= 0)
            {
                Die();
            }
        }
    }
}
