using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;


namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] bool isPlayerSpeaking = false;//will need to be reworked as an enum for multiple speakers beyond player and one entity
        [SerializeField] private string speakerName;
        [SerializeField] private string speakerJob;
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rect = new Rect(0, 0, 200, 100); //default node size
        [SerializeField] private string onEnterAction;
        [SerializeField] private string onExitAction;//can be turned into an array to trigger multiple actions
        [SerializeField] Condition condition;
        public Rect GetRect()
        {
            return rect;
        }
        public string GetSpeakerName()
        {
            return speakerName;
        }
        public string GetSpeakerJob()
        {
            return speakerJob;
        }
        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }
        public string GetOnEnterAction()
        {
            return onEnterAction;
        }
        public string GetOnExitAction()
        {
            return onExitAction;
        }

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }
        
#if UNITY_EDITOR//only runs this part in editor
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);//Remember to set dirty if adding any more
        }

        public void SetSpeakerName(string newSpeakerName)
        {
            Undo.RecordObject(this, "Changed Speaker Name");
            if(newSpeakerName!= speakerName)
            {
                Undo.RecordObject(this, "Updated Speaker Name");
                speakerName = newSpeakerName;
                EditorUtility.SetDirty(this);
            }
        }
        public void SetSpeakerJob(string newSpeakerJob)
        {
            Undo.RecordObject(this, "Changed Speaker Name");
            if(newSpeakerJob!= speakerJob)
            {
                Undo.RecordObject(this, "Updated Speaker Name");
                speakerJob = newSpeakerJob;
                EditorUtility.SetDirty(this);
            }
        }
        public void SetText(string newText)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            if(newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void SetPlayerSpeaking(bool newIsPlayerSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isPlayerSpeaking = newIsPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Removed Dialogue Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }
        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }
#endif//ends what hash began, meaning later code can still run in game
    }

}