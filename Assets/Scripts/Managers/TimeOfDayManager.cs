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

public class TimeOfDayManager : MonoBehaviour
{
    public static TimeOfDayManager Instance { get; private set; }

    [Header("Day Duration")]
    [Tooltip("How long one full day lasts in real seconds")]
    [SerializeField] private float dayDurationSeconds = 1800f;

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

    public event Action OnDayEnded;

    private float _elapsed;
    private bool _dayActive;
    private bool _dayEndTriggered;
    private int _totalInGameMinutes;
    private int _lastDisplayedMinute = -1;

    private int TotalInGameMinutes => (endHour * 60 + endMinute) - (startHour * 60 + startMinute);

    public bool IsDayActive => _dayActive;
    public bool DayEndTriggered => _dayEndTriggered;
    public float NormalizedTime => Mathf.Clamp01(_elapsed / dayDurationSeconds);

    public int CurrentHour => (startHour * 60 + startMinute + _totalInGameMinutes) / 60;
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

        float t = Mathf.Clamp01(_elapsed / dayDurationSeconds);
        _totalInGameMinutes = Mathf.FloorToInt(t * TotalInGameMinutes);

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

    public void StartDay()
    {
        _elapsed = 0f;
        _totalInGameMinutes = 0;
        _lastDisplayedMinute = 0;
        _dayActive = true;
        _dayEndTriggered = false;
        UpdateClockDisplay();
    }

    public void PauseDay() => _dayActive = false;

    public void ResumeDay()
    {
        if (!_dayEndTriggered)
            _dayActive = true;
    }

    public void SetClockText(TextMeshProUGUI text)
    {
        clockText = text;
        UpdateClockDisplay();
    }

    public void RegisterShopExit(GameObject obj)
    {
        shopExitObject = obj;
    }

    private void UpdateClockDisplay()
    {
        if (clockText == null) return;

        int totalMin = startHour * 60 + startMinute + _totalInGameMinutes;
        int h = totalMin / 60;
        int m = totalMin % 60;

        clockText.text = $"{h:D2}:{m:D2}";
    }

    private void EndDay()
    {
        _dayActive = false;
        _dayEndTriggered = true;

        if (clockText != null)
            clockText.text = $"{endHour:D2}:{endMinute:D2}";

        OnDayEnded?.Invoke();

        IsQuotaVisit = true;

        bool alreadyInShop = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == shopSceneName;
        if (alreadyInShop)
        {
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
            HandleQuotaCheckInShop();
            return;
        }

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

        LoadShopForQuota();
    }

    private System.Collections.IEnumerator WaitForDialogueThenCheckQuota()
    {
        yield return null;
        while (DialogueManager.Instance != null && DialogueManager.Instance.IsActive)
            yield return null;
        HandleQuotaCheckInShop();
    }

    private void HandleQuotaCheckInShop()
    {
        bool canAfford = QuotaManager.Instance != null && QuotaManager.Instance.CanAffordQuota();
        if (canAfford)
        {
            DisableShopExit();
        }
        else
        {
            GameOverPending = true;
            PlayCantAffordDialogue();
        }
    }

    public bool GameOverPending { get; private set; }

    public void ClearGameOverPending() => GameOverPending = false;

    public void SetGameOverPending() => GameOverPending = true;

    public void PlayCantAffordDialogue()
    {
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

    public bool IsQuotaVisit { get; private set; }

    public void MeetQuota()
    {
        if (QuotaManager.Instance != null)
            QuotaManager.Instance.PayQuota();

        IsQuotaVisit = false;
        _dayEndTriggered = false;
        DayManager.Instance.AdvanceDay();
        SceneManager.Instance.LoadScene("World");
    }
}
