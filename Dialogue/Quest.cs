using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving.Inventories;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "GameDev Testing/Quest", order = 0)]
    public class Quest : ScriptableObject 
    {
        [SerializeField] List<Objective> objectives = new List<Objective>();
        [SerializeField] List<Reward> rewards = new List<Reward>();//Reward of quest, multiple can be added

        [System.Serializable] public class Reward//class used for generating quest reward/s (system allows multiple rewards)
        {
            [Min(1)] public int number;//Number of the item given to player. Cannot give player 0 or less of any reward with this system.
            public InventoryItem item;//Item given to player
        }

        [System.Serializable] public class Objective
        {
            public string reference;//Simple reference that is used for quest triggering, not shown to player, and which can be as easy as "1" or "A"
            public string description;//Longer description of task that is shown on player's quest tooltips
        }
        public string GetTitle()
        {
            return name;
        }
        
        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        public bool HasObjective(string objectiveRef)
        {
            foreach(var objective in objectives)
            {
                if(objective.reference == objectiveRef)
                {
                    return true;
                }
            }
            return false;
        }

        public static Quest GetByName(string questName)
        {
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if(quest.name == questName)
                {
                    return quest;
                }
            }
            return null;
        }
    }
}
