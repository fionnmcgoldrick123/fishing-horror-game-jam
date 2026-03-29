using UnityEngine;

public class ShopExitRegistrar : MonoBehaviour
{
    void Start()
    {
        if (TimeOfDayManager.Instance == null) return;

        TimeOfDayManager.Instance.RegisterShopExit(gameObject);

        if (!TimeOfDayManager.Instance.IsQuotaVisit) return;

        bool canAfford = QuotaManager.Instance != null && QuotaManager.Instance.CanAffordQuota();

        if (canAfford)
        {
            gameObject.SetActive(false);
        }
        else
        {
            TimeOfDayManager.Instance.SetGameOverPending();
            TimeOfDayManager.Instance.PlayCantAffordDialogue();
        }
    }
}
