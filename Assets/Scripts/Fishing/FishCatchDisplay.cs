using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishCatchDisplay : MonoBehaviour
{
    public static FishCatchDisplay Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject catchPanel;
    [SerializeField] private RectTransform catchPanelRect;
    [SerializeField] private Image fishPortrait;
    [SerializeField] private TextMeshProUGUI fishNameText;
    [SerializeField] private TextMeshProUGUI fishDescriptionText;
    [SerializeField] private TextMeshProUGUI fishValueText;
    [SerializeField] private Image[] starImages = new Image[4];

    [Header("Star Visuals")]
    [SerializeField] private Color activeStarColor = Color.yellow;
    [SerializeField] private Color inactiveStarColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    [Header("Animation Timing")]
    [SerializeField] private float preDelay = 0.6f;
    [SerializeField] private float panelPopDuration = 0.3f;
    [SerializeField] private float descriptionCharsPerSecond = 60f;
    [SerializeField] private float countUpDuration = 0.8f;
    [SerializeField] private float starInterval = 0.25f;
    [SerializeField] private float starPunchDuration = 0.2f;

    private enum DisplayPhase { PreDelay, PanelPop, TypingDescription, CountingValue, PoppingStars, WaitingToClose, Inactive }
    private DisplayPhase phase = DisplayPhase.Inactive;

    private Coroutine activeCoroutine;
    private FishScriptableObject currentFish;
    private int currentStarCount;
    private PlayerController player;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (phase == DisplayPhase.Inactive || phase == DisplayPhase.PreDelay || phase == DisplayPhase.PanelPop) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    public void ShowCatch(FishScriptableObject fish)
    {
        currentFish = fish;
        currentStarCount = GetStarCount(fish.rarity);

        // Freeze player
        if (player != null)
            player.ChangeState(player.TalkState);

        // Start the pre-delay phase (panel stays hidden)
        phase = DisplayPhase.PreDelay;
        AudioManager.Instance?.PlayPreDelay();
        activeCoroutine = StartCoroutine(PreDelayRoutine());
    }

    private IEnumerator PreDelayRoutine()
    {
        yield return new WaitForSeconds(preDelay);
        activeCoroutine = null;
        BeginPanelPop();
    }

    private void BeginPanelPop()
    {
        // Prepare UI content before showing
        fishPortrait.sprite = currentFish.fishSprite;
        fishNameText.text = currentFish.fishName;
        fishDescriptionText.text = "";
        fishValueText.text = "0";

        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].color = inactiveStarColor;
            starImages[i].transform.localScale = Vector3.zero;
        }

        catchPanel.SetActive(true);
        AudioManager.Instance?.PlayFishCaught();

        phase = DisplayPhase.PanelPop;
        activeCoroutine = UIAnimations.PopInPanel(this, catchPanelRect, panelPopDuration, OnPanelPopComplete);
    }

    private void OnPanelPopComplete()
    {
        activeCoroutine = null;
        BeginDescription();
    }

    private void BeginDescription()
    {
        phase = DisplayPhase.TypingDescription;
        if (string.IsNullOrEmpty(currentFish.fishDescription))
        {
            OnDescriptionComplete();
            return;
        }
        activeCoroutine = UIAnimations.Typewriter(this, fishDescriptionText, currentFish.fishDescription, descriptionCharsPerSecond, OnDescriptionComplete);
    }

    private void OnDescriptionComplete()
    {
        activeCoroutine = null;
        BeginCountUp();
    }

    private void BeginCountUp()
    {
        phase = DisplayPhase.CountingValue;

        if (currentFish.value == 0)
        {
            fishValueText.text = "0";
            OnCountUpComplete();
            return;
        }

        activeCoroutine = UIAnimations.CountUp(this, fishValueText, currentFish.value, countUpDuration, OnCountUpComplete, () => AudioManager.Instance?.PlayMoneyTick());
    }

    private void OnCountUpComplete()
    {
        activeCoroutine = null;
        BeginStars();
    }

    private void BeginStars()
    {
        phase = DisplayPhase.PoppingStars;
        activeCoroutine = UIAnimations.PopInSequence(this, starImages, currentStarCount, activeStarColor, inactiveStarColor, starInterval, starPunchDuration, OnStarsComplete, () => AudioManager.Instance?.PlayStarPop());
    }

    private void OnStarsComplete()
    {
        activeCoroutine = null;
        phase = DisplayPhase.WaitingToClose;
    }

    private void HandleClick()
    {
        switch (phase)
        {
            case DisplayPhase.TypingDescription:
                if (activeCoroutine != null) StopCoroutine(activeCoroutine);
                activeCoroutine = null;
                UIAnimations.FinishTypewriter(fishDescriptionText, currentFish.fishDescription);
                OnDescriptionComplete();
                break;

            case DisplayPhase.CountingValue:
                if (activeCoroutine != null) StopCoroutine(activeCoroutine);
                activeCoroutine = null;
                UIAnimations.FinishCountUp(fishValueText, currentFish.value);
                OnCountUpComplete();
                break;

            case DisplayPhase.PoppingStars:
                if (activeCoroutine != null) StopCoroutine(activeCoroutine);
                activeCoroutine = null;
                UIAnimations.FinishPopIn(starImages, currentStarCount, activeStarColor, inactiveStarColor);
                OnStarsComplete();
                break;

            case DisplayPhase.WaitingToClose:
                Hide();
                break;
        }
    }

    public void Hide()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = null;

        catchPanel.SetActive(false);
        phase = DisplayPhase.Inactive;

        if (currentFish != null)
            FishInventory.Instance.AddFish(currentFish);

        currentFish = null;

        if (player != null)
            player.ExitFishing();
    }

    private int GetStarCount(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => 1,
            Rarity.Rare => 2,
            Rarity.Epic => 3,
            Rarity.Legendary => 4,
            _ => 1
        };
    }
}
