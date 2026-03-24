using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Place in BOTH World and Shop scenes.
/// Re-registers scene UI text with the persistent managers every time a scene loads.
/// Only starts the day timer when entering World for a fresh day.
/// </summary>
public class DayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI moneyText;

    void Start()
    {
        // Re-bind clock text (old reference was destroyed with the previous scene)
        if (TimeOfDayManager.Instance != null && clockText != null)
            TimeOfDayManager.Instance.SetClockText(clockText);

        // Re-bind money text (same reason)
        if (FishInventory.Instance != null && moneyText != null)
            FishInventory.Instance.SetMoneyText(moneyText);

        // Only start a fresh day in World when no day is running
        // and the previous day has been resolved via MeetQuota
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "World"
            && TimeOfDayManager.Instance != null
            && !TimeOfDayManager.Instance.IsDayActive
            && !TimeOfDayManager.Instance.DayEndTriggered)
        {
            TimeOfDayManager.Instance.StartDay();
        }

        UpdateDayText();
    }

    private void UpdateDayText()
    {
        if (dayText != null && DayManager.Instance != null)
            dayText.text = $"Day {DayManager.Instance.CurrentDay}";
    }
}
