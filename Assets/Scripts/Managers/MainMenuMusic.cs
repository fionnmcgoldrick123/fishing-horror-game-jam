using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    public static MainMenuMusic Instance { get; private set; }

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

    public void DestroyMusic()
    {
        Destroy(gameObject);
    }
}
