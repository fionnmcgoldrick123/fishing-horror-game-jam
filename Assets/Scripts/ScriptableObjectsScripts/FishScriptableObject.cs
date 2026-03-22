using UnityEngine;

public static class FishRarityWeights
{
    public const int Common    = 50;
    public const int Rare      = 10;
    public const int Epic      = 3;
    public const int Legendary = 1;
}

// Enum for rarity
public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "FishScriptableObject", menuName = "Scriptable Objects/FishScriptableObject")]
public class FishScriptableObject : ScriptableObject
{
    public string fishName;
    [TextArea(2, 4)] public string fishDescription;
    public Sprite fishSprite;
    public int value;
    public Rarity rarity;
    public bool isUpgradeCurrency;

    public int rarityWeight => rarity switch
    {
        Rarity.Common    => FishRarityWeights.Common,
        Rarity.Rare      => FishRarityWeights.Rare,
        Rarity.Epic      => FishRarityWeights.Epic,
        Rarity.Legendary => FishRarityWeights.Legendary,
        _ => 0
    };
}
