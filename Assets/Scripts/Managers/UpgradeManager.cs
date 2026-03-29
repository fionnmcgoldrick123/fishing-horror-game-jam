public enum UpgradeType { Luck, CatchSpeed, Ease }

public static class UpgradeManager
{
    public const int MaxLevel = 5;

    private static int _luckLevel;
    private static int _catchSpeedLevel;
    private static int _easeLevel;

    public static int LuckLevel => _luckLevel;
    public static int CatchSpeedLevel => _catchSpeedLevel;
    public static int EaseLevel => _easeLevel;

    public static int GetLevel(UpgradeType type) => type switch
    {
        UpgradeType.Luck => _luckLevel,
        UpgradeType.CatchSpeed => _catchSpeedLevel,
        UpgradeType.Ease => _easeLevel,
        _ => 0
    };

    public static bool IsMaxed(UpgradeType type) => GetLevel(type) >= MaxLevel;

    public static bool TryPurchase(UpgradeType type)
    {
        if (IsMaxed(type)) return false;
        if (FishInventory.Instance == null) return false;
        if (FishInventory.Instance.EyeballCount <= 0) return false;

        FishInventory.Instance.SpendEyeball();

        switch (type)
        {
            case UpgradeType.Luck: _luckLevel++; break;
            case UpgradeType.CatchSpeed: _catchSpeedLevel++; break;
            case UpgradeType.Ease: _easeLevel++; break;
        }

        return true;
    }

    public static float GetLuckMultiplier() => 1f + _luckLevel * 0.4f;

    public static float GetWaitTimeMultiplier() => 1f - _catchSpeedLevel * 0.12f;

    public static float GetEaseHitSizeBonus() => _easeLevel * 12f;

    public static float GetEaseSpeedMultiplier() => 1f - _easeLevel * 0.06f;

    public static float GetEaseScoreMultiplier() => 1f - _easeLevel * 0.08f;
}
