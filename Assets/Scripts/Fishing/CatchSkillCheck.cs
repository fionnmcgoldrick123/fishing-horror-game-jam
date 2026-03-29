    using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatchSkillCheck : MonoBehaviour
{
    [Header("Arrow")]
    [SerializeField] private RectTransform arrow;
    [SerializeField] private float radius;

    [Header("Speed (fallback if no DifficultyManager)")]
    [SerializeField] private float minRotationSpeed;
    [SerializeField] private float maxRotationSpeed;
    private float rotationSpeed;

    [Header("Hit Zone (fallback if no DifficultyManager)")]
    [SerializeField] private float minHitSize;
    [SerializeField] private float maxHitSize;
    [SerializeField] private Image _hitZoneImg;

    [Header("Score (fallback if no DifficultyManager)")]
    [SerializeField] private int minScore;
    [SerializeField] private int maxScore;
    [SerializeField] private TextMeshProUGUI requiredScoreTXT;

    private int direction = -1;
    private float randomStartHitZone;
    private float randomEndHitZone;
    private float hitZoneSize;
    private float currentAngle = 0f;
    private bool @lock = false;
    private int requiredScore;

    private PlayerController player;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    void OnEnable()
    {
        @lock = false;
        currentAngle = 90f;
        direction = Random.value > 0.5f ? 1 : -1;

        ApplyDifficulty();

        _hitZoneImg.type = Image.Type.Filled;
        _hitZoneImg.fillMethod = Image.FillMethod.Radial360;
        _hitZoneImg.fillOrigin = (int)Image.Origin360.Right;
        _hitZoneImg.fillClockwise = true;

        requiredScoreTXT.text = $"{requiredScore}";
        GetRandomHitZone();

        AudioManager.Instance?.StartReeling();
    }

    void Update()
    {
        MoveArrow();
        CheckInput();
    }

    void CheckInput()
    {
        if (Input.GetMouseButtonDown(0) && !@lock)
        {
            @lock = true;

            float normalised = currentAngle % 360f;
            if (normalised < 0) normalised += 360f;

            bool inHitZone;
            if (randomEndHitZone < randomStartHitZone)
                inHitZone = (normalised >= randomStartHitZone || normalised <= randomEndHitZone);
            else
                inHitZone = (normalised >= randomStartHitZone && normalised <= randomEndHitZone);

            if (inHitZone)
                HandleHit();
            else
                HandleMiss();
        }
    }

    void HandleMiss()
    {
        Debug.Log("Miss!");
        AudioManager.Instance?.StopReeling();
        AudioManager.Instance?.PlayMinigameMiss();
        HookManager.Instance.OnMinigameLost();
        player.ExitFishing();
    }

    void HandleHit()
    {
        Debug.Log("Hit!");
        AudioManager.Instance?.PlayMinigameHit();
        requiredScore -= 1;
        requiredScoreTXT.text = $"{requiredScore}";

        if (requiredScore <= 0)
        {
            HandleWin();
            return;
        }

        @lock = false;
        hitZoneSize = Random.Range(minHitSize, maxHitSize);
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        if (Random.value > 0.3f)
            direction *= -1;
        GetRandomHitZone();
    }

    void HandleWin()
    {
        Debug.Log("Win!");
        AudioManager.Instance?.StopReeling();
        HookManager.Instance.OnMinigameWon();
    }

    void GetRandomHitZone()
    {
        randomStartHitZone = Random.Range(0f, 360f);
        randomEndHitZone = (randomStartHitZone + hitZoneSize) % 360f;
        UpdateHitZoneVisual();
    }

    void UpdateHitZoneVisual()
    {
        _hitZoneImg.fillAmount = hitZoneSize / 360f;
        _hitZoneImg.rectTransform.localRotation = Quaternion.Euler(0, 0, randomStartHitZone + hitZoneSize);
    }

    void ApplyDifficulty()
    {
        float usedMinHitSize = minHitSize;
        float usedMaxHitSize = maxHitSize;
        int usedMinScore = minScore;
        int usedMaxScore = maxScore;
        float usedMinSpeed = minRotationSpeed;
        float usedMaxSpeed = maxRotationSpeed;

        if (DifficultyManager.Instance != null && HookManager.Instance.CurrentFish != null)
        {
            DifficultySettings settings = DifficultyManager.Instance.GetSettings(HookManager.Instance.CurrentFish.rarity);
            usedMinScore = settings.MinScore;
            usedMaxScore = settings.MaxScore;
            usedMinHitSize = settings.MinHitSize;
            usedMaxHitSize = settings.MaxHitSize;
            usedMinSpeed = settings.MinSpeed;
            usedMaxSpeed = settings.MaxSpeed;
        }

        hitZoneSize = Random.Range(usedMinHitSize, usedMaxHitSize);
        requiredScore = Random.Range(usedMinScore, usedMaxScore + 1);
        rotationSpeed = Random.Range(usedMinSpeed, usedMaxSpeed);
    }

    void MoveArrow()
    {
        currentAngle += rotationSpeed * Time.deltaTime * direction;
        currentAngle %= 360f;

        float rad = currentAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * radius;
        float y = Mathf.Sin(rad) * radius;

        arrow.localPosition = new Vector3(x, y, 0);
        arrow.localRotation = Quaternion.Euler(0, 0, currentAngle - 90f);
    }
}
