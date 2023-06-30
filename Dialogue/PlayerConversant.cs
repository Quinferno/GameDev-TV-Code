using System.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPG.Core;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        NPCInteractable currentConversant = null;
        bool isChoosing = false;
        public bool isTalking = false;

        public event Action onConversationUpdated;
        public void StartDialogue (NPCInteractable newConversant, Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();

            // Cursor.lockState = CursorLockMode.None;//Frees Cursor, may switch to .Confined instead at some point
            // Cursor.visible = true;//Shows cursor
            isTalking = true;
        }

        public void Quit()
        {
            TriggerExitAction();
            currentConversant = null;
            currentDialogue = null;
            currentNode = null;
            isChoosing = false;
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
            onConversationUpdated();

            isTalking = false;
        }

        private void Update()
        {
            // if (isTalking == true)
            // {
            //     Cursor.lockState = CursorLockMode.None;//Frees Cursor, may switch to .Confined instead at some point
            //     Cursor.visible = true;//Shows cursor
            // }
        }

        public bool IsActive()//makes sure there is a current dialogue
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()//Used to see if Player is making dialogue choices or not
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.GetText();
        }
        public string GetName()
        {
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.GetSpeakerName();
        }
        public string GetJob()
        {
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.GetSpeakerJob();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            onConversationUpdated();//not necessary if Next is not commented out
            //Next(); This line would allow the skipping of the node that the option came from. Don't want this by default, as I want to implement truncated options that represent longer PC responses
        }

        public void Next()
        {
            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayerResponses > 0)//Originally was 0
            {
                isChoosing = true;//makes it impossible for random dialogue IF two or more player dialogue options come next, which is a small limitation
                TriggerExitAction();//may need to add this to SelectChoice but am not yet sure
                onConversationUpdated();
                return;
            }

            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();//Still allows for player character to speak if only one option is available

            int randomIndex = UnityEngine.Random.Range(0, children.Count());//When multiple options exist, chooses totally by random
            TriggerExitAction();
            currentNode = children[randomIndex];//When multiple options exist, chooses totally by random
            TriggerEnterAction();
            onConversationUpdated();
        }

        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0; //Returns true if count is higher than 0 and false if there are no more children
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode) //important for filtering out nodes that don't make sense in current context
        {
            foreach (var node in inputNode)
            {
                if(node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();//will want components of AI being talked to in future as well
        }

        private void TriggerEnterAction()
        {
            if(currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if(currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if(action == "") return;//If action is equal to empty string, do nothing.

            foreach(DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
    }
}
