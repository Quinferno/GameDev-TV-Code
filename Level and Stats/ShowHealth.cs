using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class ShowHealth : MonoBehaviour
    {
        Health health;

        [SerializeField] bool showMaxHealth = true;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            if(showMaxHealth)
            {
            GetComponent<TMP_Text>().text = String.Format("{0:0}", health.maxHealth.value);//Remember TMP_Text is different than Text
            }

            else
            GetComponent<TMP_Text>().text = String.Format("{0:0}", health.health.value);
        }
    }
}
