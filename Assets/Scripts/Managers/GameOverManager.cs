using System.Runtime.CompilerServices;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [SerializeField] private DialogueData gameOverDialogue;
    [SerializeField] private Vector3 newDrSturgeonPosition;

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

    public void TriggerGameOver()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(gameOverDialogue);
        }
        else
        {
            Debug.LogError("GameOverManager: DialogueManager instance not found. Cannot display game over dialogue.");
        }
    }
}
