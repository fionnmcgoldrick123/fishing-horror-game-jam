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

        [Header("References")]
        [SerializeField] private AudioSource _reelInSound;

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

            _hitZoneSize = Random.Range(_minHitSize, _maxHitSize);
            _requiredScore = Random.Range(_minScore, _maxScore);
            _rotationSpeed = Random.Range(_minRotationSpeed, _maxRotationSpeed);

            GetRandomHitZone();
        }

        void Update()
        {
            CheckWinCondition();
            MoveArrow();
            CheckInput();

            _requiredScoreTXT.text = $"{_requiredScore}";
        }

        void CheckInput()
        {
            if (Input.GetMouseButtonDown(0) && !_lock)
            {
                _lock = true;

                if (_currentAngle < 0) _currentAngle += 360f;

                bool inHitZone;
                if (_randomEndHitZone < _randomStartHitZone)
                    inHitZone = (_currentAngle >= _randomStartHitZone || _currentAngle <= _randomEndHitZone);
                else
                    inHitZone = (_currentAngle >= _randomStartHitZone && _currentAngle <= _randomEndHitZone);

                if (inHitZone)
                    HandleHit();
                else
                    HandleMiss();
            }
        }

        void HandleMiss()
        {
            gameObject.SetActive(false);
            _reelInSound.Stop();
        }

        void HandleHit()
        {
            _lock = false;
            _requiredScore -= 1;
            _rotationSpeed = Random.Range(_minRotationSpeed, _maxRotationSpeed);
            GetRandomHitZone();
            _direction *= -1;
        }

        void CheckWinCondition()
        {
            if (_requiredScore <= 0)
            {
                _reelInSound.Stop();
            }
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
            _hitZoneImg.rectTransform.localRotation = Quaternion.Euler(0, 0, _randomStartHitZone);
        }

        void MoveArrow()
        {
            _currentAngle += _rotationSpeed * Time.deltaTime * _direction;
            _currentAngle %= 360f;

            float rad = _currentAngle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * radius;
            float y = Mathf.Sin(rad) * radius;

            _arrow.localPosition = new Vector3(x, y, 0);
            _arrow.localRotation = Quaternion.Euler(0, 0, _currentAngle + 270);
        }
    }
