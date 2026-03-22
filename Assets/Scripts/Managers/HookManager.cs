using UnityEngine;

public class HookManager : MonoBehaviour
{
    public static HookManager Instance { get; private set; }

    [SerializeField] private GameObject fishingMinigameUI;
    [SerializeField] private FishingSystem fishingSystem;

    [Header("Wait Timer")]
    [SerializeField] private float minWaitTime = 3f;
    [SerializeField] private float maxWaitTime = 10f;

    [Header("Alert Window")]
    [SerializeField] private float alertDuration = 1.5f;
    [SerializeField] private GameObject alertUI;

    private enum HookState { Idle, Waiting, Alerting, Minigame }
    private HookState state = HookState.Idle;
    private float timer;

    private FishScriptableObject currentFish;
    public FishScriptableObject CurrentFish => currentFish;
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
        float multiplier = UpgradeManager.GetWaitTimeMultiplier();
        timer = Random.Range(minWaitTime, maxWaitTime) * multiplier;
        currentFish = null;
        if (alertUI != null) alertUI.SetActive(false);
    }

    /// <summary>
    /// Called when the player leaves fishing entirely. Resets everything.
    /// </summary>
    public void StopWaiting()
    {
        state = HookState.Idle;
        timer = 0f;
        currentFish = null;
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

    /// <summary>
    /// Called by CatchSkillCheck when the player wins the minigame.
    /// </summary>
    public void OnMinigameWon()
    {
        fishingMinigameUI.SetActive(false);
        state = HookState.Idle;

        if (currentFish != null)
        {
            FishCatchDisplay.Instance.ShowCatch(currentFish);
        }

        currentFish = null;
    }

    /// <summary>
    /// Called by CatchSkillCheck when the player fails the minigame.
    /// </summary>
    public void OnMinigameLost()
    {
        fishingMinigameUI.SetActive(false);
        state = HookState.Idle;
        Debug.Log(currentFish != null ? $"Lost: {currentFish.fishName} got away!" : "Fish got away!");
        currentFish = null;
    }

    private void BeginAlert()
    {
        state = HookState.Alerting;
        timer = alertDuration;

        // Determine which fish is on the line
        currentFish = fishingSystem.GetRandomFish();

        if (alertUI != null) alertUI.SetActive(true);
        AudioManager.Instance?.PlayHookAlert();
    }

    private void ExpireAlert()
    {
        if (alertUI != null) alertUI.SetActive(false);

        // Missed the bite — go back to waiting for another one
        StartWaiting();
    }
}
