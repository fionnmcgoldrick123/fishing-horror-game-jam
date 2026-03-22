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

    /// <summary>
    /// Attempts to purchase an upgrade. Returns true if successful.
    /// Costs one eyeball per purchase.
    /// </summary>
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

    // Luck: multiplier on rare+ fish weight (higher = more rare fish)
    public static float GetLuckMultiplier() => 1f + _luckLevel * 0.25f;

    // Catch Speed: wait-time multiplier (lower = shorter wait)
    public static float GetWaitTimeMultiplier() => 1f - _catchSpeedLevel * 0.12f;

    // Ease: extra hit-zone size (degrees)
    public static float GetEaseHitSizeBonus() => _easeLevel * 8f;

    // Ease: speed subtracted from rotation speed
    public static float GetEaseSpeedReduction() => _easeLevel * 15f;

    // Ease: required-score reduction (min clamped to 1 at point of use)
    public static int GetEaseScoreReduction() => _easeLevel;
}
