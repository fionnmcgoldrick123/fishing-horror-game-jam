using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.Anim.SetFloat("xVelocity", 0f);
    }

    public override void Update()
    {
        if (player.HorizontalInput != 0)
        {
            player.ChangeState(player.WalkState);
            return;
        }

        if (player.CanFish && Input.GetKeyDown(KeyCode.E))
        {
            player.ChangeState(player.FishingIdleState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocityY);
    }
}
