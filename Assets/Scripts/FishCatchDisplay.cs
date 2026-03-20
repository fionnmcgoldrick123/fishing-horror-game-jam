using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishCatchDisplay : MonoBehaviour
{
    public static FishCatchDisplay Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject catchPanel;
    [SerializeField] private Image fishPortrait;
    [SerializeField] private TextMeshProUGUI fishNameText;
    [SerializeField] private TextMeshProUGUI fishValueText;
    [SerializeField] private Image[] starImages = new Image[4];

    [Header("Star Visuals")]
    [SerializeField] private Color activeStarColor = Color.yellow;
    [SerializeField] private Color inactiveStarColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ShowCatch(FishScriptableObject fish)
    {
        fishPortrait.sprite = fish.fishSprite;
        fishNameText.text = fish.fishName;
        fishValueText.text = $"{fish.value}g";

        int starCount = GetStarCount(fish.rarity);
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].color = i < starCount ? activeStarColor : inactiveStarColor;
        }

        catchPanel.SetActive(true);
    }

    public void Hide()
    {
        catchPanel.SetActive(false);
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
