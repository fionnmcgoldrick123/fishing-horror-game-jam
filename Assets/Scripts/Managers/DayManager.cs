using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    public int CurrentDay { get; private set; } = 1;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AdvanceDay()
    {
        QuotaManager.Instance.IncreaseQuota();
        CurrentDay++;
    }

    public void Reset()
    {
        CurrentDay = 1;
    }
}
