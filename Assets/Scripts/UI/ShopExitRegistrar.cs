using UnityEngine;

/// <summary>
/// Attach this to the exit GameObject in the shop scene.
/// On quota visits it checks whether the player can afford the quota:
///   - Can afford  → exit disabled, player must pay quota at the shop.
///   - Can't afford → "can't afford" dialogue plays, exit stays enabled so they can leave.
/// </summary>
public class ShopExitRegistrar : MonoBehaviour
{
    void Start()
    {
        if (TimeOfDayManager.Instance == null) return;

        // Always register so TimeOfDayManager can disable/enable us later.
        TimeOfDayManager.Instance.RegisterShopExit(gameObject);

        if (!TimeOfDayManager.Instance.IsQuotaVisit) return;

        // --- Quota-visit logic ---
        bool canAfford = QuotaManager.Instance != null && QuotaManager.Instance.CanAffordQuota();

        if (canAfford)
        {
            // Lock exit — player must go to shop counter and pay
            gameObject.SetActive(false);
        }
        else
        {
            // Can't afford — arm the game-over sequence and play the fail dialogue.
            // PlayCantAffordDialogue also sets GameOverPending internally, but we set
            // it here explicitly too in case the call order ever changes.
            TimeOfDayManager.Instance.SetGameOverPending();
            TimeOfDayManager.Instance.PlayCantAffordDialogue();
        }
    }
}
