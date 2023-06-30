using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Quests;
using System;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;
        [SerializeField]TextMeshProUGUI rewardText;
        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();

            title.text = quest.GetTitle();

            foreach(Transform item in objectiveContainer)
            {
                Destroy(item.gameObject);
            }

            foreach (var objective in quest.GetObjectives())
            {
                GameObject prefab = objectiveIncompletePrefab;
                if(status.IsObjectiveComplete(objective.reference))
                {
                    prefab = objectivePrefab;
                }
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.description;
            }
            rewardText.text = GetRewardText(quest);
        }

        private string GetRewardText(Quest quest)
        {
            string rewardText = "";
            foreach(var reward in quest.GetRewards())
            {
                if(rewardText != "")//Used to help make sure multiple items get noted in the reward section with commas
                {
                    rewardText += ", ";
                }
                if(reward.number > 1)
                {
                    rewardText += reward.number + " ";//Displays number, one space, and then item name if player gets multiple
                }
                rewardText += reward.item.GetDisplayName();
            }

            if(rewardText == "")
            {
                rewardText = "No Reward Offered";
            }
            rewardText += "";//Originally was a period, adding a period to end of item list, but I think it looks better without
            return rewardText;
        }
    }
}