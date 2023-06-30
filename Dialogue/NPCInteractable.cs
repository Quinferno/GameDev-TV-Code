using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using System;

public class NPCInteractable : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject player;
    private Animator animator;
    public PlayerConversant playerConversant;
    public bool interactionOccurred;
    public event Action<NPCInteractable> OnDestroyed;//Added for OnDestroy and TalkTargeter

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if(animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if(interactionOccurred == true)
        {
        interactionOccurred = false;
        }
    }
    public void Interact()
    {
        interactionOccurred = true;
        playerConversant.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);   
        animator.SetTrigger("Talk");//Triggers "Talk" in animator, although this can be changed to be any kind of animator trigger for different interaction types

        if(playerConversant.isTalking == false)//for NPCs with statemachines
        {
        interactionOccurred = true;
        playerConversant.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);   
        animator.SetTrigger("Talk");//Triggers "Talk" in animator, although this can be changed to be any kind of animator trigger for different interaction types
        }
    }

    public void Speak() 
    {
        player.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);    
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }
}
