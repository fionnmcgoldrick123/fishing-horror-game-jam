using UnityEngine;

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
                player.ShopSceneName = sceneName;
                break;
            case InteractionType.ExitShop:
                player.CanExitShop = true;
                player.SetExitSpawnPosition(exitSpawnPosition);
                break;
            case InteractionType.FishingZone:
                player.CanFish = true;
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
                break;
            case InteractionType.ExitShop:
                player.CanExitShop = false;
                break;
            case InteractionType.FishingZone:
                player.CanFish = false;
                break;
        }
    }
}
