using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Attributes;
using UnityEngine;

public class GetWeaponDamage : MonoBehaviour
{
    [SerializeField] IEnumerable<ElementalDamage> elementalDamages;
    GameObject player;
    Fighter playerFighter;
    WeaponConfig currentWeaponConfig;
    float damage;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerFighter = player.GetComponent<Fighter>();
        
    }

    void Update()
    {
        currentWeaponConfig = playerFighter.currentWeaponConfig;
        //elementalDamages = currentWeaponConfig.GetElementalDamages();
    }
}
