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

    public static bool IsOpen { get; private set; }

    private Coroutine animCoroutine;
    private Coroutine bgCoroutine;
    private bool isOpen;

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

        UpdateSellPreview();
    }

    public void CloseShop()
    {
        if (!isOpen) return;
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
