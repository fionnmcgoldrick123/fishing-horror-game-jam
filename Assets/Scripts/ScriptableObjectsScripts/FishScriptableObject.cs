using UnityEngine;

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
    public Sprite fishSprite;
    public int value;
    public Rarity rarity;
}
