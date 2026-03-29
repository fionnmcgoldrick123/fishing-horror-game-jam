using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Fishing Sprites")]
    [SerializeField] private Sprite fishingIdleSprite;
    [SerializeField] private Sprite fishingPrepSprite;

    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; private set; }
    public SpriteRenderer Sr { get; private set; }
    public float MoveSpeed => moveSpeed;
    public float HorizontalInput { get; private set; }
    public bool CanFish { get; set; }
    public bool CanEnterShop { get; set; }
    public bool CanExitShop { get; set; }
    public string ShopSceneName { get; set; }

    private Vector3 _exitSpawnPosition;

    public Sprite FishingIdleSprite => fishingIdleSprite;
    public Sprite FishingPrepSprite => fishingPrepSprite;

    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerTalkState TalkState { get; private set; }
    public PlayerFishingState FishingState { get; private set; }

    [SerializeField] private GameObject interactionPrompt;

    private PlayerState currentState;

    void Awake()
    {

        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();
        Sr = GetComponentInChildren<SpriteRenderer>();

        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        TalkState = new PlayerTalkState(this);
        FishingState = new PlayerFishingState(this);
    }

    void Start()
    {
        ChangeState(IdleState);
    }

    void Update()
    {
        HorizontalInput = ShopButtonManager.IsOpen ? 0f : Input.GetAxisRaw("Horizontal");
        currentState?.Update();
        CheckInteractionInput();
    }

    private void CheckInteractionInput()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (ShopButtonManager.IsOpen) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive) return;

        if (CanEnterShop && !string.IsNullOrEmpty(ShopSceneName))
        {
            AudioManager.Instance.PlayOpeningDoor();
            SceneManager.Instance.LoadScene(ShopSceneName);
        }
        else if (CanExitShop)
        {
            AudioManager.Instance.PlayOpeningDoor();
            SceneManager.Instance.LoadSceneWithSpawn("World", _exitSpawnPosition);
        }
    }

    void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void HandleFlip()
    {
        if (HorizontalInput > 0 && transform.localScale.x < 0)
            Flip();
        else if (HorizontalInput < 0 && transform.localScale.x > 0)
            Flip();
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;

        if (interactionPrompt != null)
        {
            Vector3 promptScale = interactionPrompt.transform.localScale;
            promptScale.x *= -1f;
            interactionPrompt.transform.localScale = promptScale;
        }
    }

    public void SetExitSpawnPosition(Vector3 position) => _exitSpawnPosition = position;

    public void ExitFishing() => ChangeState(IdleState);

    public void EnterDialogue(DialogueData data)
    {
        ChangeState(TalkState);
        TimeOfDayManager.Instance?.PauseDay();
        DialogueManager.Instance.StartDialogue(data);
    }

    public void ExitDialogue()
    {
        ChangeState(IdleState);
        TimeOfDayManager.Instance?.ResumeDay();
    }

    public void LockMovement() => ChangeState(TalkState);

    public void ShowInteractionPrompt()
    {
        if (interactionPrompt.TryGetComponent(out TextMeshProUGUI tmp))
        {
            tmp.color = Color.white;
        }
        interactionPrompt.SetActive(true);
    }

    public void ShowInteractionPromptBlack()
    {
        if (interactionPrompt.TryGetComponent(out TextMeshProUGUI tmp))
        {
            tmp.color = Color.black;
        }
        interactionPrompt.SetActive(true);
    }

    public void HideInteractionPrompt() => interactionPrompt.SetActive(false);

}
