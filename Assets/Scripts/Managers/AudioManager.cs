using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;

    [Header("Fishing")]
    [SerializeField] private AudioClip hookAlert;
    [SerializeField] private AudioClip bobberLand;
    [SerializeField] private AudioClip reeling;

    [Header("Minigame")]
    [SerializeField] private AudioClip minigameHit;
    [SerializeField] private AudioClip minigameMiss;

    [Header("Catch Display")]
    [SerializeField] private AudioClip preDelay;
    [SerializeField] private AudioClip fishCaught;
    [SerializeField] private AudioClip moneyTick;
    [SerializeField] private AudioClip starPop;

    [Header("Inventory / Score")]
    [SerializeField] private AudioClip scoreTick;

    [Header("Shop")]
    [SerializeField] private AudioClip sellFish;
    [SerializeField] private AudioClip upgradePurchase;
    [SerializeField] private AudioClip upgradeFail;
    [SerializeField] private AudioClip quotaFail;
    [SerializeField] private AudioClip openingDoor;

    public void PlayOpeningDoor() => Play(openingDoor);
    public void PlayQuotaFail() => Play(quotaFail);

    [Header("Dialogue")]
    [SerializeField] private AudioClip dialogueTyping;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ───── One-shot sounds ─────

    public void PlayHookAlert() => Play(hookAlert);
    public void PlayBobberLand() => Play(bobberLand);
    public void PlayMinigameHit() => Play(minigameHit);
    public void PlayMinigameMiss() => Play(minigameMiss);
    public void PlayPreDelay() => Play(preDelay);
    public void PlayFishCaught() => Play(fishCaught);
    public void PlayMoneyTick() => Play(moneyTick);
    public void PlayStarPop() => Play(starPop);
    public void PlayScoreTick() => Play(scoreTick);
    public void PlaySellFish() => Play(sellFish);
    public void PlayUpgradePurchase() => Play(upgradePurchase);
    public void PlayUpgradeFail() => Play(upgradeFail);
    public void PlayDialogueTyping() => Play(dialogueTyping);

    /// <summary>
    /// Plays a DialogueData's custom typing sound if it has one, otherwise the default.
    /// </summary>
    public void PlayDialogueTyping(AudioClip overrideClip)
    {
        Play(overrideClip != null ? overrideClip : dialogueTyping);
    }

    // ───── Looping sounds ─────

    public void StartReeling()
    {
        if (loopSource == null || reeling == null) return;
        loopSource.clip = reeling;
        loopSource.loop = true;
        loopSource.Play();
    }

    public void StopReeling()
    {
        if (loopSource == null || !loopSource.isPlaying) return;
        loopSource.Stop();
        loopSource.loop = false;
        loopSource.clip = null;
    }

    // ───── Core ─────

    private void Play(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
