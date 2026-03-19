    using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatchSkillCheck : MonoBehaviour
{
    [Header("Arrow")]
    [SerializeField] private RectTransform _arrow;
    [SerializeField] private float radius;

    [Header("Speed")]
    [SerializeField] private float _minRotationSpeed;
    [SerializeField] private float _maxRotationSpeed;
    private float _rotationSpeed;

    [Header("Hit Zone")]
    [SerializeField] private float _minHitSize;
    [SerializeField] private float _maxHitSize;
    [SerializeField] private Image _hitZoneImg;

    [Header("Score")]
    [SerializeField] private int _minScore;
    [SerializeField] private int _maxScore;
    [SerializeField] private TextMeshProUGUI _requiredScoreTXT;

    private int _direction = -1;
    private float _randomStartHitZone;
    private float _randomEndHitZone;
    private float _hitZoneSize;
    private float _currentAngle = 0f;
    private bool _lock = false;
    private int _requiredScore;

    void OnEnable()
    {
        _lock = false;
        _currentAngle = 90f;
        _direction = Random.value > 0.5f ? 1 : -1;

        _hitZoneSize = Random.Range(_minHitSize, _maxHitSize);
        _requiredScore = Random.Range(_minScore, _maxScore);
        _rotationSpeed = Random.Range(_minRotationSpeed, _maxRotationSpeed);

        // Force correct Image fill settings so the arc displays properly
        _hitZoneImg.type = Image.Type.Filled;
        _hitZoneImg.fillMethod = Image.FillMethod.Radial360;
        _hitZoneImg.fillOrigin = (int)Image.Origin360.Right;
        _hitZoneImg.fillClockwise = true;

        _requiredScoreTXT.text = $"{_requiredScore}";
        GetRandomHitZone();
    }

    void Update()
    {
        MoveArrow();
        CheckInput();
    }

    void CheckInput()
    {
        if (Input.GetMouseButtonDown(0) && !_lock)
        {
            _lock = true;

            float normalised = _currentAngle % 360f;
            if (normalised < 0) normalised += 360f;

            bool inHitZone;
            if (_randomEndHitZone < _randomStartHitZone)
                inHitZone = (normalised >= _randomStartHitZone || normalised <= _randomEndHitZone);
            else
                inHitZone = (normalised >= _randomStartHitZone && normalised <= _randomEndHitZone);

            if (inHitZone)
                HandleHit();
            else
                HandleMiss();
        }
    }

    void HandleMiss()
    {
        Debug.Log("Miss!");
        gameObject.SetActive(false);
    }

    void HandleHit()
    {
        Debug.Log("Hit!");
        _requiredScore -= 1;
        _requiredScoreTXT.text = $"{_requiredScore}";

        if (_requiredScore <= 0)
        {
            HandleWin();
            return;
        }

        _lock = false;
        _rotationSpeed = Random.Range(_minRotationSpeed, _maxRotationSpeed);
        _direction *= -1;
        GetRandomHitZone();
    }

    void HandleWin()
    {
        gameObject.SetActive(false);
    }

    void GetRandomHitZone()
    {
        _randomStartHitZone = Random.Range(0f, 360f);
        _randomEndHitZone = (_randomStartHitZone + _hitZoneSize) % 360f;
        UpdateHitZoneVisual();
    }

    void UpdateHitZoneVisual()
    {
        _hitZoneImg.fillAmount = _hitZoneSize / 360f;
        // Rotate to the end angle so the clockwise fill sweeps back to _randomStartHitZone
        _hitZoneImg.rectTransform.localRotation = Quaternion.Euler(0, 0, _randomStartHitZone + _hitZoneSize);
    }

    void MoveArrow()
    {
        _currentAngle += _rotationSpeed * Time.deltaTime * _direction;
        _currentAngle %= 360f;

        float rad = _currentAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * radius;
        float y = Mathf.Sin(rad) * radius;

        _arrow.localPosition = new Vector3(x, y, 0);
        _arrow.localRotation = Quaternion.Euler(0, 0, _currentAngle - 90f);
    }
}
