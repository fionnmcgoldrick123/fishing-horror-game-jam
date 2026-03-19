    using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatchSkillCheck : MonoBehaviour
{
    [Header("Arrow")]
    [SerializeField] private RectTransform arrow;
    [SerializeField] private float radius;

    [Header("Speed")]
    [SerializeField] private float minRotationSpeed;
    [SerializeField] private float maxRotationSpeed;
    private float rotationSpeed;

    [Header("Hit Zone")]
    [SerializeField] private float minHitSize;
    [SerializeField] private float maxHitSize;
    [SerializeField] private Image _hitZoneImg;

    [Header("Score")]
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

        hitZoneSize = Random.Range(minHitSize, maxHitSize);
        requiredScore = Random.Range(minScore, maxScore);
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);

        // Force correct Image fill settings so the arc displays properly
        _hitZoneImg.type = Image.Type.Filled;
        _hitZoneImg.fillMethod = Image.FillMethod.Radial360;
        _hitZoneImg.fillOrigin = (int)Image.Origin360.Right;
        _hitZoneImg.fillClockwise = true;

        requiredScoreTXT.text = $"{requiredScore}";
        GetRandomHitZone();
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
        player.ExitFishing();
        gameObject.SetActive(false);
    }

    void HandleHit()
    {
        Debug.Log("Hit!");
        requiredScore -= 1;
        requiredScoreTXT.text = $"{requiredScore}";

        if (requiredScore <= 0)
        {
            HandleWin();
        }

        @lock = false;
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        direction *= -1;
        GetRandomHitZone();
    }

    void HandleWin()
    {
        Debug.Log("Win!");
        player.ExitFishing();
        gameObject.SetActive(false);
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
        // Rotate to the end angle so the clockwise fill sweeps back to _randomStartHitZone
        _hitZoneImg.rectTransform.localRotation = Quaternion.Euler(0, 0, randomStartHitZone + hitZoneSize);
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
