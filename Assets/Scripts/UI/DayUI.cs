using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Place in BOTH World and Shop scenes.
/// Registers clock/day text with TimeOfDayManager every time a scene loads.
/// Only starts the day timer when in the World scene.
/// </summary>
public class DayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private TextMeshProUGUI dayText;

    void Start()
    {
        if (TimeOfDayManager.Instance != null)
        {
            // Always register the clock text so the manager can update it
            if (clockText != null)
                TimeOfDayManager.Instance.SetClockText(clockText);

            // Only start (or restart) the timer in the World scene
            if (SceneManager.GetActiveScene() == "World")
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
