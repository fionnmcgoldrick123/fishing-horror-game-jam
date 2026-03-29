using UnityEngine;
using TMPro;

public class ShopButtonManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private GameObject shopBackground;

    [Header("Animation")]
    [SerializeField] private float popInDuration = 0.3f;
    [SerializeField] private float popOutDuration = 0.15f;
    [SerializeField] private float backgroundFadeDuration = 0.2f;

    [Header("Sell Feedback")]
    [SerializeField] private TextMeshProUGUI sellTotalText;
    [SerializeField] private float countUpDuration = 0.8f;

    [Header("Quota")]
    [SerializeField] private TextMeshProUGUI quotaAmountText;
    [Tooltip("The Meet Quota button — assign so it can be disabled immediately on press to prevent spam.")]
    [SerializeField] private UnityEngine.UI.Button meetQuotaButton;

    public static bool IsOpen { get; private set; }

    private Coroutine animCoroutine;
    private Coroutine bgCoroutine;
    private bool isOpen;

    private void Start()
    {
        quotaAmountText.text = QuotaManager.Instance != null ? QuotaManager.Instance.currentQuota.ToString("F0") : "N/A";
    }

    public void OpenShop()
    {
        if (isOpen) return;

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive)
            DialogueManager.Instance.ForceClose();

        isOpen = true;
        IsOpen = true;
        TimeOfDayManager.Instance?.PauseDay();
        {
            shopBackground.gameObject.SetActive(true);
            if (bgCoroutine != null) StopCoroutine(bgCoroutine);
            bgCoroutine = StartCoroutine(FadeBackground(0f, 1f, backgroundFadeDuration));
        }

        shopPanel.gameObject.SetActive(true);

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = UIAnimations.PopInPanel(this, shopPanel, popInDuration);

        UpdateSellPreview();
    }

    public void CloseShop()
    {
        if (!isOpen) return;

        if (TimeOfDayManager.Instance != null && TimeOfDayManager.Instance.IsQuotaVisit
            && QuotaManager.Instance != null && QuotaManager.Instance.CanAffordQuota()) return;

        isOpen = false;
        IsOpen = false;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(PopOutRoutine());
    }

    private void UpdateSellPreview()
    {
        if (sellTotalText == null || FishInventory.Instance == null) return;
        sellTotalText.text = FishInventory.Instance.TotalValue.ToString();
    }

    public void OnMeetQuotaPressed()
    {
        if (QuotaManager.Instance == null || !QuotaManager.Instance.CanAffordQuota())
        {
            AudioManager.Instance?.PlayQuotaFail();
            return;
        }

        if (meetQuotaButton != null)
            meetQuotaButton.interactable = false;

        isOpen = false;
        IsOpen = false;

        StartCoroutine(MeetQuotaSequence());
    }

    private System.Collections.IEnumerator MeetQuotaSequence()
    {
        if (bgCoroutine != null) StopCoroutine(bgCoroutine);
        bgCoroutine = StartCoroutine(FadeBackground(1f, 0f, popOutDuration));

        float elapsed = 0f;
        while (elapsed < popOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / popOutDuration);
            shopPanel.localScale = new Vector3(Mathf.Lerp(1f, 0f, t * t), Mathf.Lerp(1f, 0f, t * t), 1f);
            yield return null;
        }
        shopPanel.localScale = Vector3.zero;
        shopPanel.gameObject.SetActive(false);
        if (shopBackground != null)
            shopBackground.gameObject.SetActive(false);

        DayLoader dayLoader = FindFirstObjectByType<DayLoader>();
        if (dayLoader != null)
            dayLoader.StartTransition();
        else if (TimeOfDayManager.Instance != null)
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

        TimeOfDayManager.Instance?.ResumeDay();
    }

    private System.Collections.IEnumerator FadeBackground(float from, float to, float duration)
    {
        if (shopBackground == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
