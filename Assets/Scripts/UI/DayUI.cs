using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI moneyText;

    void Start()
    {
        if (TimeOfDayManager.Instance != null && clockText != null)
            TimeOfDayManager.Instance.SetClockText(clockText);

        if (FishInventory.Instance != null && moneyText != null)
            FishInventory.Instance.SetMoneyText(moneyText);

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
