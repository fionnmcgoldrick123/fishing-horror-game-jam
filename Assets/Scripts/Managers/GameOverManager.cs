using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("Game Over Sound")]
    [Tooltip("Sound that plays when the jumpscare triggers in the World scene.")]
    [SerializeField] private AudioClip gameOverSound;
    [Tooltip("Extra seconds to wait after the sound finishes before loading the Game Over scene.")]
    [SerializeField] private float pauseAfterSound = 1.5f;

    [Header("Scenes")]
    [Tooltip("The exact name of the World scene.")]
    [SerializeField] private string worldSceneName = "World";
    [Tooltip("The exact name of the Game Over scene.")]
    [SerializeField] private string gameOverSceneName = "GameOver";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != worldSceneName) return;
        if (TimeOfDayManager.Instance == null || !TimeOfDayManager.Instance.GameOverPending) return;

        TimeOfDayManager.Instance.ClearGameOverPending();
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return null;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        player?.LockMovement();
        player?.HideInteractionPrompt();

        DrSturgeonGameOver drSturgeon = FindFirstObjectByType<DrSturgeonGameOver>();
        if (drSturgeon != null)
            drSturgeon.MoveToJumpscarePosition();
        else
            Debug.LogWarning("GameOverManager: DrSturgeonGameOver component not found in World scene. " +
                             "Add the DrSturgeonGameOver component to Dr. Sturgeon and set his Jumpscare Position.");

        float soundDuration = 0f;
        if (AudioManager.Instance != null && gameOverSound != null)
            soundDuration = AudioManager.Instance.PlayClipGetLength(gameOverSound);
        else if (gameOverSound == null)
            Debug.LogWarning("GameOverManager: No game over sound assigned.");

        yield return new WaitForSeconds(soundDuration + pauseAfterSound);

        if (global::SceneManager.Instance != null)
            global::SceneManager.Instance.LoadScene(gameOverSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameOverSceneName);
    }

    public void TriggerGameOver()
    {
        TimeOfDayManager.Instance?.SetGameOverPending();
    }
}
