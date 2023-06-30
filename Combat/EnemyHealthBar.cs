using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class EnemyHealthBar : MonoBehaviour
    {
        public Health health;
        public Slider slider;
        [SerializeField] bool dontToggle = false;
        //Fighter playerFighter;

        [SerializeField] GameObject root;
        private float maxHealth;
        private float currentHealth;

        public void Start()
        {
            maxHealth = health.GetCurrentMaxHealth();
            currentHealth = health.GetCurrentHealth();
            if (dontToggle == false)
            {
            root.SetActive(false);//have mixed feelings about starting this off
            }
            //playerFighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }
        public void Update ()
        {
            // if(playerFighter.GetTarget() == null && dontToggle == false)//Inelegant but should work to turn the bar off when player has no target
            // {
            //     gameObject.transform.GetChild(0).gameObject.SetActive(false);
            //     gameObject.transform.GetChild(1).gameObject.SetActive(false);
            //     return;
            // }
            
            //Health health = playerFighter.GetTarget();//Enemy info comes from the target the player has selected

            // if(playerFighter.GetTarget() == true && dontToggle == false)//Inelegant but should work to turn the bar off when player has no target
            // {
            //     gameObject.transform.GetChild(0).gameObject.SetActive(true);
            //     gameObject.transform.GetChild(1).gameObject.SetActive(true);
            // }

            currentHealth = health.GetCurrentHealth();

            slider.maxValue = maxHealth;
            slider.value = currentHealth;

            bool isDead = health.IsDead();
            bool isHurt = health.IsHurt();

            if(isHurt == true)
            {
                root.SetActive(true);
            }

            if(isDead == true)
            {
                root.SetActive(false);
            }
        }
        
    }
}
