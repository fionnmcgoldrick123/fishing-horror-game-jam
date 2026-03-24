using UnityEngine;

/// <summary>
/// Attach this to the GameObject in the shop scene that the player uses to leave
/// (e.g. the exit door or leave button).
///
/// At end of day the object is automatically disabled, preventing the player from leaving.
/// Works whether the player was already in the shop or was loaded into it from the world.
/// </summary>
public class ShopExitRegistrar : MonoBehaviour
{
    void Start()
    {
        if (TimeOfDayManager.Instance == null) return;

        // Register ourselves so TimeOfDayManager can disable us after end-of-day dialogue.
        TimeOfDayManager.Instance.RegisterShopExit(gameObject);

        // If the player was sent here from the world at end of day, dialogue already
        // finished before the scene loaded — disable immediately.
        if (TimeOfDayManager.Instance.IsQuotaVisit)
            gameObject.SetActive(false);
    }
}
