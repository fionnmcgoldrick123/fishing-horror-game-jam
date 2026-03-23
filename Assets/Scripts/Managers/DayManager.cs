using UnityEngine;

/// <summary>
/// Tracks the current day number. Persists across scenes.
/// </summary>
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
        CurrentDay++;
    }
}
