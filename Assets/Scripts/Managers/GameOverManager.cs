using System.Runtime.CompilerServices;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [SerializeField] private DialogueData gameOverDialogue;
    [SerializeField] private Vector3 newDrSturgeonPosition;
    [SerializeField] private GameObject drSturgeonNPC;
    [SerializeField] private AudioClip gameOverSound;

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
        // Move Dr. Sturgeon to the new position if reference is set
        if (drSturgeonNPC != null)
        {
            drSturgeonNPC.transform.position = newDrSturgeonPosition;
        }
        else
        {
            Debug.LogWarning("GameOverManager: Dr. Sturgeon NPC reference not set.");
        }

        // Play game over sound if audio clip is set
        if (gameOverSound != null && AudioManager.Instance != null)
        {
            // AudioManager.Instance.PlaySoundEffect(gameOverSound);
        }
        else if (gameOverSound == null)
        {
            Debug.LogWarning("GameOverManager: Game over sound clip not assigned.");
        }

        // Start the game over dialogue
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
