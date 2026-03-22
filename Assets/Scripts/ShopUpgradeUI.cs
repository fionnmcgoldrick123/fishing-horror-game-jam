using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUpgradeUI : MonoBehaviour
{
    [Header("Eyeball Currency")]
    [SerializeField] private TextMeshProUGUI eyeballCountText;

    [Header("Luck Upgrade")]
    [SerializeField] private Button luckButton;
    [SerializeField] private Image[] luckStars = new Image[5];

    [Header("Catch Speed Upgrade")]
    [SerializeField] private Button catchSpeedButton;
    [SerializeField] private Image[] catchSpeedStars = new Image[5];

    [Header("Ease Upgrade")]
    [SerializeField] private Button easeButton;
    [SerializeField] private Image[] easeStars = new Image[5];

    [Header("Star Colors")]
    [SerializeField] private Color litStarColor = Color.yellow;
    [SerializeField] private Color unlitStarColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip purchaseClip;
    [SerializeField] private AudioClip failClip;

    void OnEnable()
    {
        RefreshAll();
    }

    public void OnLuckButtonClicked()
    {
        TryPurchase(UpgradeType.Luck, luckStars);
    }

    public void OnCatchSpeedButtonClicked()
    {
        TryPurchase(UpgradeType.CatchSpeed, catchSpeedStars);
    }

    public void OnEaseButtonClicked()
    {
        TryPurchase(UpgradeType.Ease, easeStars);
    }

    private void TryPurchase(UpgradeType type, Image[] stars)
    {
        if (UpgradeManager.TryPurchase(type))
        {
            PlayClip(purchaseClip);
            RefreshAll();
        }
        else
        {
            PlayClip(failClip);
        }
    }

    public void RefreshAll()
    {
        UpdateStars(luckStars, UpgradeManager.LuckLevel);
        UpdateStars(catchSpeedStars, UpgradeManager.CatchSpeedLevel);
        UpdateStars(easeStars, UpgradeManager.EaseLevel);
        UpdateEyeballCount();
    }

    private void UpdateStars(Image[] stars, int level)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = i < level ? litStarColor : unlitStarColor;
        }
    }

    private void UpdateEyeballCount()
    {
        if (eyeballCountText != null && FishInventory.Instance != null)
            eyeballCountText.text = FishInventory.Instance.EyeballCount.ToString();
    }

    private void PlayClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
