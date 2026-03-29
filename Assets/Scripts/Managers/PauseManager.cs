using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel;

    [Header("Audio")]
    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip unpauseSound;
    private AudioSource audioSource;

    [Header("Volume")]
    [SerializeField] private Slider masterVolumeSlider;

    public static bool IsPaused { get; private set; }

    private const string VolumeKey = "MasterVolume";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = savedVolume;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = savedVolume;
            masterVolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        pausePanel?.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                Resume();
            else if (DialogueManager.Instance == null || !DialogueManager.Instance.IsActive)
                Pause();
        }
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        if (pauseSound != null && audioSource != null)
            audioSource.PlayOneShot(pauseSound);
        pausePanel?.SetActive(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        if (unpauseSound != null && audioSource != null)
            audioSource.PlayOneShot(unpauseSound);
        pausePanel?.SetActive(false);
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
    }
}
