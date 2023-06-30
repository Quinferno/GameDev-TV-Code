using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        public Health health;
        public Slider slider;
        private float maxHealth;
        private float currentHealth;

        public void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }
        public void Update ()
        {
            slider.maxValue = health.maxHealth.value;
            slider.value = health.health.value;
        }
        
    }
}
