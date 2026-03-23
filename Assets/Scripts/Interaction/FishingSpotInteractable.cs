using UnityEngine;

public class FishingSpotInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.ChangeState(player.FishingState);
    }
}
