using UnityEngine;

public enum CharacterID { DrSturgeon, Perch, Car}

public class NPCDialogueTrigger : MonoBehaviour
{
    [Tooltip("Which character this NPC is — used by DialogueSwappingManager to assign the correct day dialogue.")]
    [SerializeField] private CharacterID characterId;
    [SerializeField] private DialogueData dialogueData;
    [Tooltip("If true, the shop panel opens automatically when this dialogue ends (use for Perch).")]
    [SerializeField] private bool opensShopOnEnd;

    public CharacterID CharacterId => characterId;

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
