using UnityEngine;

// Attach to an NPC GameObject with a Collider2D set to Is Trigger.
// Assign a DialogueData asset in the Inspector.
public class NPCDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueData;
    private PlayerController player;

    private bool _playerInRange;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (!_playerInRange) return;
        if (ShopButtonManager.IsOpen) return;
        if (DialogueManager.Instance == null) return;
        if (DialogueManager.Instance.IsActive) return;
        if (DialogueManager.Instance.JustEnded()) return;
        if (!Input.GetKeyDown(KeyCode.E)) return;

        player.EnterDialogue(dialogueData);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = false;
    }
}
