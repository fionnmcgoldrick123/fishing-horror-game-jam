using UnityEngine;

public enum CharacterID { DrSturgeon, Perch, Car}

// Attach to an NPC GameObject with a Collider2D set to Is Trigger.
// Assign a DialogueData asset in the Inspector.
public class NPCDialogueTrigger : MonoBehaviour
{
    [Tooltip("Which character this NPC is — used by DialogueSwappingManager to assign the correct day dialogue.")]
    [SerializeField] private CharacterID characterId;
    [SerializeField] private DialogueData dialogueData;
    [Tooltip("If true, the shop panel opens automatically when this dialogue ends (use for Perch).")]
    [SerializeField] private bool opensShopOnEnd;

    public CharacterID CharacterId => characterId;

    /// <summary>Called by DialogueSwappingManager to swap this NPC's dialogue for the current day.</summary>
    public void SetDialogueData(DialogueData data) { dialogueData = data; }

    private PlayerController player;

    private bool _playerInRange;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (!_playerInRange) return;
        if (DialogueManager.Instance == null) return;

        // If dialogue just ended and this NPC should open the shop, do it now
        if (opensShopOnEnd && DialogueManager.Instance.JustEnded())
        {
            ShopButtonManager shopManager = FindFirstObjectByType<ShopButtonManager>();
            shopManager?.OpenShop();
            return;
        }

        if (ShopButtonManager.IsOpen) return;
        if (DialogueManager.Instance.IsActive) return;
        if (DialogueManager.Instance.JustEnded()) return;
        if (!Input.GetKeyDown(KeyCode.E)) return;

        player.EnterDialogue(dialogueData);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            player.ShowInteractionPrompt();
            _playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            player.HideInteractionPrompt();
            _playerInRange = false;
    }
}
