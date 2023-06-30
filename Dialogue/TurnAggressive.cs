using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using UnityEngine;

public class TurnAggressive : Fighter
{
    [SerializeField] Fighter nPCFighter;
    public void Update()
    {
        // nPCFighter.IsAggressive = true;
    }
}
