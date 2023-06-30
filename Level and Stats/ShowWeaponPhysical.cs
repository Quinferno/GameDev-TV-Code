using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RPG.Combat;

namespace RPG.Attributes
{
    public class ShowWeaponPhysical : MonoBehaviour
    {
        Fighter playerFighter;
        WeaponConfig currentWeaponConfig;
        float weaponPhysicalDamage = 0;

        private void Awake()
        {
            // playerFighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            // currentWeaponConfig = playerFighter.currentWeaponConfig;
            // weaponPhysicalDamage = currentWeaponConfig.GetDamage();
        }

        private void Update()
        {
            // weaponPhysicalDamage = currentWeaponConfig.GetDamage();
            // GetComponent<TMP_Text>().text = String.Format("{0:0}", weaponPhysicalDamage);
        }
    }
}
