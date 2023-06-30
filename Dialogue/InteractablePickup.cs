using UnityEngine;
using RPG.Dialogue;
using RPG.Saving.Inventories;

[RequireComponent(typeof(Pickup))]
public class InteractablePickup : MonoBehaviour
{
    public PlayerConversant playerConversant;
    public bool interactionOccurred;
    Pickup pickup;
    private void Awake()
    {
        pickup = GetComponent<Pickup>();
    }

    public void Interact()
    {
        pickup.PickupItem();
    }
}
