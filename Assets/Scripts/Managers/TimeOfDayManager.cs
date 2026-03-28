using System;
using UnityEngine;
using TMPro;

public enum TimeInterval
{
    OneMinute = 1,
    FiveMinutes = 5,
    TenMinutes = 10,
    ThirtyMinutes = 30,
    OneHour = 60
}

/// <summary>
/// Manages in-game time of day, translating real seconds into a clock display.
/// Fires an event when the day ends.
/// </summary>
public class TimeOfDayManager : MonoBehaviour
{
    public static TimeOfDayManager Instance { get; private set; }

    [Header("Day Duration")]
    [Tooltip("How long one full day lasts in real seconds")]
    [SerializeField] private float dayDurationSeconds = 1800f; // 30 minutes

    [Header("Clock Range")]
    [Tooltip("In-game hour the day starts at (0-23)")]
    [SerializeField] private int startHour = 6;
    [Tooltip("In-game minute the day starts at (0-59)")]
    [SerializeField] private int startMinute = 0;
    [Tooltip("In-game hour the day ends at (0-23)")]
    [SerializeField] private int endHour = 22;
    [Tooltip("In-game minute the day ends at (0-59)")]
    [SerializeField] private int endMinute = 0;

    [Header("Display")]
    [Tooltip("How much in-game time passes per clock tick")]
    [SerializeField] private TimeInterval displayInterval = TimeInterval.TenMinutes;
    [Tooltip("TextMeshProUGUI to display the clock. Assign per-scene or leave null.")]
    [SerializeField] private TextMeshProUGUI clockText;

    [Header("Day End")]
    [Tooltip("Dialogue to play when the day ends")]
    [SerializeField] private DialogueData dayEndDialogue;
    [Tooltip("Dialogue to play when the player can't afford the quota")]
    [SerializeField] private DialogueData cantAffordQuotaDialogue;
    [Tooltip("Scene to load after day-end dialogue")]
    [SerializeField] private string shopSceneName = "Shop";
    [Tooltip("Disable this object when the player is locked in the shop at end of day (e.g. the exit door/button)")]
    private GameObject shopExitObject;

    /// <summary>Fired when the day timer runs out.</summary>
    public event Action OnDayEnded;

    private float _elapsed;
    private bool _dayActive;
    private bool _dayEndTriggered;
    private int _totalInGameMinutes;
    private int _lastDisplayedMinute = -1;

    // The full in-game time range in minutes
    private int TotalInGameMinutes => (endHour * 60 + endMinute) - (startHour * 60 + startMinute);

    public bool IsDayActive => _dayActive;
    public bool DayEndTriggered => _dayEndTriggered;
    public float NormalizedTime => Mathf.Clamp01(_elapsed / dayDurationSeconds);

    /// <summary>Current in-game hour (e.g. 14 for 2 PM)</summary>
    public int CurrentHour => (startHour * 60 + startMinute + _totalInGameMinutes) / 60;
    /// <summary>Current in-game minute (e.g. 30)</summary>
    public int CurrentMinute => (startHour * 60 + startMinute + _totalInGameMinutes) % 60;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!_dayActive || _dayEndTriggered) return;

        _elapsed += Time.deltaTime;

        // Calculate current in-game time in minutes
        float t = Mathf.Clamp01(_elapsed / dayDurationSeconds);
        _totalInGameMinutes = Mathf.FloorToInt(t * TotalInGameMinutes);

        // Snap to display interval
        int interval = (int)displayInterval;
        int snapped = (_totalInGameMinutes / interval) * interval;
        if (snapped != _lastDisplayedMinute)
        {
            _lastDisplayedMinute = snapped;
            UpdateClockDisplay();
        }

        if (_elapsed >= dayDurationSeconds)
        {
            EndDay();
        }
    }

    /// <summary>Call this to start the day timer (e.g. when World scene loads).</summary>
    public void StartDay()
    {
        _elapsed = 0f;
        _totalInGameMinutes = 0;
        _lastDisplayedMinute = 0;
        _dayActive = true;
        _dayEndTriggered = false;
        UpdateClockDisplay();
    }

    /// <summary>Call to pause the timer (e.g. during cutscenes).</summary>
    public void PauseDay() => _dayActive = false;

    /// <summary>Call to resume a paused timer.</summary>
    public void ResumeDay()
    {
        // Do not resume if the day has already ended — prevents the timer from
        // accidentally restarting when CloseShop is called from the end-of-day shop scene.
        if (!_dayEndTriggered)
            _dayActive = true;
    }

    /// <summary>Assign a clock text from the current scene's UI.</summary>
    public void SetClockText(TextMeshProUGUI text)
    {
        clockText = text;
        UpdateClockDisplay();
    }

    /// <summary>Called by ShopExitRegistrar when the shop scene loads.</summary>
    public void RegisterShopExit(GameObject obj)
    {
        shopExitObject = obj;
    }

    private void UpdateClockDisplay()
    {
        if (clockText == null) return;

        // Use _totalInGameMinutes for accurate display (avoids -1 edge case on start)
        int totalMin = startHour * 60 + startMinute + _totalInGameMinutes;
        int h = totalMin / 60;
        int m = totalMin % 60;

        clockText.text = $"{h:D2}:{m:D2}";
    }

    private void EndDay()
    {
        _dayActive = false;
        _dayEndTriggered = true;

        // Snap clock to end time
        if (clockText != null)
            clockText.text = $"{endHour:D2}:{endMinute:D2}";

        OnDayEnded?.Invoke();

        // Set quota flag immediately so shop locks if player is already there
        IsQuotaVisit = true;

        bool alreadyInShop = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == shopSceneName;
        if (alreadyInShop)
        {
            // Player is already in the shop — play day-end dialogue, then check quota
            if (dayEndDialogue != null)
            {
                PlayerController player = FindFirstObjectByType<PlayerController>();
                if (player != null)
                {
                    player.EnterDialogue(dayEndDialogue);
                    StartCoroutine(WaitForDialogueThenCheckQuota());
                    return;
                }
            }
            // No dialogue — check quota immediately
            HandleQuotaCheckInShop();
            return;
        }

        // Play day-end dialogue, then load shop
        if (dayEndDialogue != null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.EnterDialogue(dayEndDialogue);
                StartCoroutine(WaitForDialogueThenLoadShop());
                return;
            }
        }

        // If no dialogue, go straight to shop
        LoadShopForQuota();
    }

    private System.Collections.IEnumerator WaitForDialogueThenCheckQuota()
    {
        yield return null;
        while (DialogueManager.Instance != null && DialogueManager.Instance.IsActive)
            yield return null;
        HandleQuotaCheckInShop();
    }

    /// <summary>
    /// After day-end dialogue finishes while already in the shop,
    /// check if the player can afford the quota.
    /// </summary>
    private void HandleQuotaCheckInShop()
    {
        bool canAfford = QuotaManager.Instance != null && QuotaManager.Instance.CanAffordQuota();
        if (canAfford)
        {
            DisableShopExit();
        }
        else
        {
            // Can't afford — flag game over and play fail dialogue; exit stays enabled
            GameOverPending = true;
            PlayCantAffordDialogue();
        }
    }

    /// <summary>True when the player failed the quota and must face game-over on returning to World.</summary>
    public bool GameOverPending { get; private set; }

    public void ClearGameOverPending() => GameOverPending = false;

    /// <summary>Marks game-over as pending (e.g. called from legacy code paths).</summary>
    public void SetGameOverPending() => GameOverPending = true;

    /// <summary>Plays the can't-afford-quota dialogue and marks game-over as pending.</summary>
    public void PlayCantAffordDialogue()
    {
        // Always stamp the flag here so any caller automatically arms the game-over sequence.
        GameOverPending = true;
        if (cantAffordQuotaDialogue == null) return;
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.EnterDialogue(cantAffordQuotaDialogue);
    }

    private void EnableShopExit()
    {
        if (shopExitObject != null)
            shopExitObject.SetActive(true);
    }

    private System.Collections.IEnumerator WaitForDialogueThenLoadShop()
    {
        // Wait one frame for dialogue to actually start
        yield return null;

        while (DialogueManager.Instance != null && DialogueManager.Instance.IsActive)
            yield return null;

        LoadShopForQuota();
    }

    private void DisableShopExit()
    {
        if (shopExitObject != null)
            shopExitObject.SetActive(false);
    }

    private void LoadShopForQuota()
    {
        SceneManager.Instance.LoadScene(shopSceneName);
    }

    /// <summary>
    /// True when the player was sent to the shop at end of day.
    /// ShopButtonManager checks this to show the quota button and block leaving.
    /// Reset when MeetQuota() is called.
    /// </summary>
    public bool IsQuotaVisit { get; private set; }

    /// <summary>
    /// Called by the Meet Quota button in the shop.
    /// Advances the day and returns the player to the world.
    /// </summary>
    public void MeetQuota()
    {
        // Deduct quota cost from player money
        if (QuotaManager.Instance != null)
            QuotaManager.Instance.PayQuota();

        IsQuotaVisit = false;
        _dayEndTriggered = false;
        DayManager.Instance.AdvanceDay();
        SceneManager.Instance.LoadScene("World");

        // Timer will be restarted when the world scene calls StartDay()
    }
}
