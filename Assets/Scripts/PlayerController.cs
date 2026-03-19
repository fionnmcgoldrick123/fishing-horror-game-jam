using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; private set; }
    public float MoveSpeed => moveSpeed;
    public float HorizontalInput { get; private set; }
    public bool CanFish { get; set; }

    // States
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerTalkState TalkState { get; private set; }
    public PlayerFishingIdleState FishingIdleState { get; private set; }
    public PlayerFishingPrepState FishingPrepState { get; private set; }
    public PlayerFishingCastState FishingCastState { get; private set; }
    public PlayerFishingWaitState FishingWaitState { get; private set; }

    private PlayerState currentState;

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();

        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        TalkState = new PlayerTalkState(this);
        FishingIdleState = new PlayerFishingIdleState(this);
        FishingPrepState = new PlayerFishingPrepState(this);
        FishingCastState = new PlayerFishingCastState(this);
        FishingWaitState = new PlayerFishingWaitState(this);
    }

    void Start()
    {
        ChangeState(IdleState);
    }

    void Update()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        currentState?.Update();
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
    }

    public void ExitFishing()
    {
        Anim.SetBool("isFishing", false);
        Anim.SetBool("isPrepping", false);
        Anim.ResetTrigger("Casting");
        Anim.Play("Blend Tree");
        ChangeState(IdleState);
    }
}
