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
        // Don't trigger if dialogue is active or just ended from previous E press
        if (_playerInRange && !ShopButtonManager.IsOpen && !DialogueManager.Instance.IsActive && !DialogueManager.Instance.JustEnded() && Input.GetKeyDown(KeyCode.E))
        {
            player.EnterDialogue(dialogueData);
        }
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
