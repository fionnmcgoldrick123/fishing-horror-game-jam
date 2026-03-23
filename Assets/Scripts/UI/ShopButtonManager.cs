using UnityEngine;
using TMPro;

public class ShopButtonManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private CanvasGroup shopBackground;

    [Header("Animation")]
    [SerializeField] private float popInDuration = 0.3f;
    [SerializeField] private float popOutDuration = 0.15f;
    [SerializeField] private float backgroundFadeDuration = 0.2f;

    [Header("Sell Feedback")]
    [SerializeField] private TextMeshProUGUI sellTotalText;
    [SerializeField] private float countUpDuration = 0.8f;

    [Header("Quota")]
    [SerializeField] private GameObject meetQuotaButton;

    public static bool IsOpen { get; private set; }

    private Coroutine animCoroutine;
    private Coroutine bgCoroutine;
    private bool isOpen;

    void Update()
    {
        // If shop is already open and quota visit kicks in (player was in shop when day ended),
        // lock it immediately by showing the quota button and hiding the ability to close.
        if (isOpen && meetQuotaButton != null && TimeOfDayManager.Instance != null && TimeOfDayManager.Instance.IsQuotaVisit)
        {
            if (!meetQuotaButton.activeSelf)
                meetQuotaButton.SetActive(true);
        }
    }

    public void OpenShop()
    {
        if (isOpen) return;

        // Close dialogue first if it's active
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive)
            DialogueManager.Instance.ForceClose();

        isOpen = true;
        IsOpen = true;

        if (shopBackground != null)
        {
            shopBackground.gameObject.SetActive(true);
            shopBackground.alpha = 0f;
            if (bgCoroutine != null) StopCoroutine(bgCoroutine);
            bgCoroutine = StartCoroutine(FadeBackground(0f, 1f, backgroundFadeDuration));
        }

        shopPanel.gameObject.SetActive(true);

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = UIAnimations.PopInPanel(this, shopPanel, popInDuration);

        // Show or hide quota button based on whether this is an end-of-day visit
        if (meetQuotaButton != null)
            meetQuotaButton.SetActive(TimeOfDayManager.Instance != null && TimeOfDayManager.Instance.IsQuotaVisit);

        UpdateSellPreview();
    }

    public void CloseShop()
    {
        if (!isOpen) return;

        // Block closing if this is a quota visit — must press Meet Quota instead
        if (TimeOfDayManager.Instance != null && TimeOfDayManager.Instance.IsQuotaVisit) return;

        isOpen = false;
        IsOpen = false;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(PopOutRoutine());
    }

    public void SellAllFish()
    {
        if (FishInventory.Instance == null) return;

        int earnings = FishInventory.Instance.SellAll();
        if (earnings <= 0) return;

        AudioManager.Instance?.PlaySellFish();

        if (sellTotalText != null)
            UIAnimations.CountUp(this, sellTotalText, earnings, countUpDuration);

        UpdateSellPreview();
    }

    private void UpdateSellPreview()
    {
        if (sellTotalText == null || FishInventory.Instance == null) return;
        sellTotalText.text = $"${FishInventory.Instance.TotalValue}";
    }

    /// <summary>
    /// Called by the Meet Quota button. Closes the shop, advances the day, returns to world.
    /// </summary>
    public void OnMeetQuotaPressed()
    {
        isOpen = false;
        IsOpen = false;

        if (TimeOfDayManager.Instance != null)
            TimeOfDayManager.Instance.MeetQuota();
    }

    private System.Collections.IEnumerator PopOutRoutine()
    {
        if (bgCoroutine != null) StopCoroutine(bgCoroutine);
        bgCoroutine = StartCoroutine(FadeBackground(1f, 0f, popOutDuration));

        float elapsed = 0f;
        while (elapsed < popOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / popOutDuration);
            float scale = Mathf.Lerp(1f, 0f, t * t);
            shopPanel.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        shopPanel.localScale = Vector3.zero;
        shopPanel.gameObject.SetActive(false);

        if (shopBackground != null)
            shopBackground.gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator FadeBackground(float from, float to, float duration)
    {
        if (shopBackground == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            shopBackground.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        shopBackground.alpha = to;
    }
}
