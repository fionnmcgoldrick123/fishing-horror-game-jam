using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Common Difficulty")]
    [SerializeField] private int commonMinScore = 1;
    [SerializeField] private int commonMaxScore = 2;
    [SerializeField] private float commonMinHitSize = 80f;
    [SerializeField] private float commonMaxHitSize = 120f;
    [SerializeField] private float commonMinSpeed = 100f;
    [SerializeField] private float commonMaxSpeed = 150f;

    [Header("Rare Difficulty")]
    [SerializeField] private int rareMinScore = 2;
    [SerializeField] private int rareMaxScore = 3;
    [SerializeField] private float rareMinHitSize = 60f;
    [SerializeField] private float rareMaxHitSize = 90f;
    [SerializeField] private float rareMinSpeed = 140f;
    [SerializeField] private float rareMaxSpeed = 200f;

    [Header("Epic Difficulty")]
    [SerializeField] private int epicMinScore = 3;
    [SerializeField] private int epicMaxScore = 5;
    [SerializeField] private float epicMinHitSize = 40f;
    [SerializeField] private float epicMaxHitSize = 70f;
    [SerializeField] private float epicMinSpeed = 180f;
    [SerializeField] private float epicMaxSpeed = 260f;

    [Header("Legendary Difficulty")]
    [SerializeField] private int legendaryMinScore = 4;
    [SerializeField] private int legendaryMaxScore = 6;
    [SerializeField] private float legendaryMinHitSize = 25f;
    [SerializeField] private float legendaryMaxHitSize = 50f;
    [SerializeField] private float legendaryMinSpeed = 220f;
    [SerializeField] private float legendaryMaxSpeed = 320f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public DifficultySettings GetSettings(Rarity rarity)
    {
        DifficultySettings settings = rarity switch
        {
            Rarity.Common => new DifficultySettings(commonMinScore, commonMaxScore, commonMinHitSize, commonMaxHitSize, commonMinSpeed, commonMaxSpeed),
            Rarity.Rare => new DifficultySettings(rareMinScore, rareMaxScore, rareMinHitSize, rareMaxHitSize, rareMinSpeed, rareMaxSpeed),
            Rarity.Epic => new DifficultySettings(epicMinScore, epicMaxScore, epicMinHitSize, epicMaxHitSize, epicMinSpeed, epicMaxSpeed),
            Rarity.Legendary => new DifficultySettings(legendaryMinScore, legendaryMaxScore, legendaryMinHitSize, legendaryMaxHitSize, legendaryMinSpeed, legendaryMaxSpeed),
            _ => new DifficultySettings(commonMinScore, commonMaxScore, commonMinHitSize, commonMaxHitSize, commonMinSpeed, commonMaxSpeed)
        };

        float hitBonus = UpgradeManager.GetEaseHitSizeBonus();
        float speedReduction = UpgradeManager.GetEaseSpeedReduction();
        int scoreReduction = UpgradeManager.GetEaseScoreReduction();

        settings.MinHitSize += hitBonus;
        settings.MaxHitSize += hitBonus;
        settings.MinSpeed = Mathf.Max(50f, settings.MinSpeed - speedReduction);
        settings.MaxSpeed = Mathf.Max(60f, settings.MaxSpeed - speedReduction);
        settings.MinScore = Mathf.Max(1, settings.MinScore - scoreReduction);
        settings.MaxScore = Mathf.Max(1, settings.MaxScore - scoreReduction);

        return settings;
    }
}

public struct DifficultySettings
{
    public int MinScore;
    public int MaxScore;
    public float MinHitSize;
    public float MaxHitSize;
    public float MinSpeed;
    public float MaxSpeed;

    public DifficultySettings(int minScore, int maxScore, float minHitSize, float maxHitSize, float minSpeed, float maxSpeed)
    {
        MinScore = minScore;
        MaxScore = maxScore;
        MinHitSize = minHitSize;
        MaxHitSize = maxHitSize;
        MinSpeed = minSpeed;
        MaxSpeed = maxSpeed;
    }
}
