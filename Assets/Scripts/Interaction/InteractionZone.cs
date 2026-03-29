using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InteractionType
{
    EnterShop,
    ExitShop,
    FishingZone
}

public class InteractionZone : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private string sceneName;
    [SerializeField] private SpriteRenderer fishingSpriteRenderer;

    [Header("Exit Shop Spawn (only used for ExitShop type)")]
    [SerializeField] private Vector3 exitSpawnPosition;

    public Vector3 ExitSpawnPosition => exitSpawnPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) == 0) return;

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        switch (interactionType)
        {
            case InteractionType.EnterShop:
                player.CanEnterShop = true;
                player.ShowInteractionPrompt();
                player.ShopSceneName = sceneName;
                break;
            case InteractionType.ExitShop:
                player.CanExitShop = true;
                player.SetExitSpawnPosition(exitSpawnPosition);
                player.ShowInteractionPrompt();
                break;
            case InteractionType.FishingZone:
                player.CanFish = true;
                player.ShowInteractionPromptBlack();
                if (fishingSpriteRenderer != null)
                    fishingSpriteRenderer.enabled = false;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) == 0) return;

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        switch (interactionType)
        {
            case InteractionType.EnterShop:
                player.CanEnterShop = false;
                player.ShopSceneName = null;
                player.HideInteractionPrompt();
                break;
            case InteractionType.ExitShop:
                player.CanExitShop = false;
                player.HideInteractionPrompt();
                break;
            case InteractionType.FishingZone:
                player.CanFish = false;
                player.HideInteractionPrompt();
                if (fishingSpriteRenderer != null)
                    fishingSpriteRenderer.enabled = true;
                break;
        }
    }
}
