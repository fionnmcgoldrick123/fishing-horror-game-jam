using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    private bool _hasSpawnOverride;
    private Vector3 _spawnOverridePosition;

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

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneWithSpawn(string sceneName, Vector3 spawnPosition)
    {
        _hasSpawnOverride = true;
        _spawnOverridePosition = spawnPosition;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!_hasSpawnOverride) return;
        _hasSpawnOverride = false;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;

        player.transform.position = _spawnOverridePosition;
        if (player.Rb != null)
            player.Rb.position = _spawnOverridePosition;
    }
}
