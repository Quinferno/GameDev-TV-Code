using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class ShowBaseDamage : MonoBehaviour
    {
        BaseStats playerStats;
        float baseDamage = 0;

        private void Awake()
        {
            playerStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            baseDamage = playerStats.GetBaseStat(Stats.PhysicalDamage);
            GetComponent<TMP_Text>().text = String.Format("{0:0}", baseDamage);
        }
    }
}
