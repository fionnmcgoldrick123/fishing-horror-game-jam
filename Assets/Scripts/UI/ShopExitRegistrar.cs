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
            // Can't afford — play dialogue, leave exit enabled so they can go
            TimeOfDayManager.Instance.PlayCantAffordDialogue();
        }
    }
}
