using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent manager that handles the game-over jumpscare sequence.
/// Lives in the first scene as a DontDestroyOnLoad object.
/// When the World scene loads and GameOverPending is true, it:
///   1. Locks the player
///   2. Finds the DrSturgeonGameOver component and teleports the NPC
///   3. Plays the jumpscare sound
///   4. Waits, then loads the GameOver scene
/// </summary>
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
        // Wait one frame so all Start() calls in the World scene have run.
        yield return null;

        // Lock the player.
        PlayerController player = FindFirstObjectByType<PlayerController>();
        player?.LockMovement();
        player?.HideInteractionPrompt();

        // Find Dr. Sturgeon and teleport him.
        DrSturgeonGameOver drSturgeon = FindFirstObjectByType<DrSturgeonGameOver>();
        if (drSturgeon != null)
            drSturgeon.MoveToJumpscarePosition();
        else
            Debug.LogWarning("GameOverManager: DrSturgeonGameOver component not found in World scene. " +
                             "Add the DrSturgeonGameOver component to Dr. Sturgeon and set his Jumpscare Position.");

        // Play the jumpscare sound and wait for it to finish.
        float soundDuration = 0f;
        if (AudioManager.Instance != null && gameOverSound != null)
            soundDuration = AudioManager.Instance.PlayClipGetLength(gameOverSound);
        else if (gameOverSound == null)
            Debug.LogWarning("GameOverManager: No game over sound assigned.");

        yield return new WaitForSeconds(soundDuration + pauseAfterSound);

        // Load the Game Over scene.
        if (global::SceneManager.Instance != null)
            global::SceneManager.Instance.LoadScene(gameOverSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameOverSceneName);
    }

    /// <summary>
    /// Called when the player fails to meet the quota.
    /// Flags the game-over sequence to run when World next loads.
    /// </summary>
    public void TriggerGameOver()
    {
        TimeOfDayManager.Instance?.SetGameOverPending();
    }
}
