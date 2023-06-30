using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus
    {
        Quest quest;
        List<string> completedObjectives = new List<string>();

        [System.Serializable] class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
        }
        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;
            quest = Quest.GetByName(state.questName);
            completedObjectives = state.completedObjectives;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedCount()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective); //Checks list for objective, returns "true" if present and "false" if not
        }

        public void CompleteObjective(string objective)
        {
            if (quest.HasObjective(objective))
            {
            completedObjectives.Add(objective);
            }
        }

        public object CaptureState()
        {
            QuestStatusRecord state = new QuestStatusRecord();
            state.questName = quest.name;
            state.completedObjectives = completedObjectives;
            return state;
        }

        public bool IsComplete()
        {
            foreach(var objective in quest.GetObjectives())
            {
                if(!completedObjectives.Contains(objective.reference))//if any one objective is labeled incomplete, quest cannot count as complete. May need override or more complexity later.
                {
                    return false;
                }
            }
            return true;
        }
    }
}
