using UnityEngine;

public class HookManager : MonoBehaviour
{
    public static HookManager Instance { get; private set; }

    [SerializeField] private GameObject fishingMinigameUI;

    [Header("Wait Timer")]
    [SerializeField] private float minWaitTime = 3f;
    [SerializeField] private float maxWaitTime = 10f;

    [Header("Alert Window")]
    [SerializeField] private float alertDuration = 1.5f;
    [SerializeField] private GameObject alertUI;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hookAlertClip;

    private enum HookState { Idle, Waiting, Alerting, Minigame }
    private HookState state = HookState.Idle;
    private float timer;

    public bool IsAlerting => state == HookState.Alerting;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (state == HookState.Waiting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                BeginAlert();
        }
        else if (state == HookState.Alerting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                ExpireAlert();
        }
    }

    /// <summary>
    /// Called when the player enters the fishing Wait phase.
    /// Starts a random countdown before a fish bites.
    /// </summary>
    public void StartWaiting()
    {
        state = HookState.Waiting;
        timer = Random.Range(minWaitTime, maxWaitTime);
        if (alertUI != null) alertUI.SetActive(false);
    }

    /// <summary>
    /// Called when the player leaves fishing entirely. Resets everything.
    /// </summary>
    public void StopWaiting()
    {
        state = HookState.Idle;
        timer = 0f;
        if (alertUI != null) alertUI.SetActive(false);
    }

    /// <summary>
    /// Called by the fishing state when the player clicks during the alert.
    /// Returns true if the click was during the alert window, then starts the minigame.
    /// </summary>
    public bool TryHook()
    {
        if (state != HookState.Alerting) return false;

        state = HookState.Minigame;
        if (alertUI != null) alertUI.SetActive(false);
        fishingMinigameUI.SetActive(true);
        return true;
    }

    public void StopMinigame()
    {
        fishingMinigameUI.SetActive(false);
        state = HookState.Idle;
    }

    private void BeginAlert()
    {
        state = HookState.Alerting;
        timer = alertDuration;

        if (alertUI != null) alertUI.SetActive(true);
        if (audioSource != null && hookAlertClip != null)
            audioSource.PlayOneShot(hookAlertClip);
    }

    private void ExpireAlert()
    {
        if (alertUI != null) alertUI.SetActive(false);

        // Missed the bite — go back to waiting for another one
        StartWaiting();
    }
}
