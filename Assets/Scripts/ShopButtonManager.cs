using UnityEngine;
using TMPro;

public class ShopButtonManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform shopPanel;

    [Header("Animation")]
    [SerializeField] private float popInDuration = 0.3f;
    [SerializeField] private float popOutDuration = 0.15f;

    [Header("Sell Feedback")]
    [SerializeField] private TextMeshProUGUI sellTotalText;
    [SerializeField] private float countUpDuration = 0.8f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sellClip;

    private Coroutine animCoroutine;
    private bool isOpen;

    /// <summary>
    /// Hook this up to your "Open Shop" button OnClick.
    /// </summary>
    public void OpenShop()
    {

        Debug.Log("OpenShop called");
        
        if (isOpen) return;
        isOpen = true;

        shopPanel.gameObject.SetActive(true);

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = UIAnimations.PopInPanel(this, shopPanel, popInDuration);

        UpdateSellPreview();
    }

    /// <summary>
    /// Hook this up to your "Back / Close" button OnClick.
    /// </summary>
    public void CloseShop()
    {
        if (!isOpen) return;
        isOpen = false;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(PopOutRoutine());
    }

    /// <summary>
    /// Hook this up to your "Sell All Fish" button OnClick.
    /// </summary>
    public void SellAllFish()
    {
        if (FishInventory.Instance == null) return;

        int earnings = FishInventory.Instance.SellAll();
        if (earnings <= 0) return;

        if (audioSource != null && sellClip != null)
            audioSource.PlayOneShot(sellClip);

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
        float elapsed = 0f;
        Vector3 startScale = shopPanel.localScale;
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
    }
}
