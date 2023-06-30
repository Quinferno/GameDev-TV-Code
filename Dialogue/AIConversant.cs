using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RPG.Control; This namespace is used in the tutorial but isn't implemented

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour//may be unnedded?
    {
        [SerializeField] private GameObject player;
        [SerializeField] Dialogue dialogue = null;//may have multiple of similar objects in the future for more complex NPCs
        public NPCInteractable nPCInteractable;

        //public CursorType GetCursorType()
        //{
        //    return CursorType.Dialogue;
        //}

        //public bool HandleRaycast(PlayerController callingController)
        //{
            // if(Dialogue == null)
            // {
            //     return false;
            // }

            // if(Input.GetMouseButtonDown(0))
            // {
            //      callingController.GetComponent<PlayerConversant>().StartDialogue(dialogue);    
            // }  
        //    return true;
        //}
        // public void Speak() 
        // {
        //     if(nPCInteractable.interactionOccurred == true)
        //     {
        //         player.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);    
        //     }  
        // }

    }
}
