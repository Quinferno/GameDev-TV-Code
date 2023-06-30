using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 5f;//probably fine as private, may need to change if interaction range can be upgraded or varies for some reason
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.E))//This should probably be changed to integrate better with rest of control system
        {
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out InteractablePickup interactablePickup))//looks for interactable objects
                {
                    interactablePickup.Interact();
                }
                if (collider.TryGetComponent(out NPCInteractable npcInteractable))//looks for interactable NPCs
                {
                    npcInteractable.Interact();
                }
            }
        }
    }
}
