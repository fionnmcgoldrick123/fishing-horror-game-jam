using UnityEngine;

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(PlayerController player) : base(player) { }

    public override void Update()
    {
        player.Anim.SetFloat("xVelocity", Mathf.Abs(player.HorizontalInput));
        player.HandleFlip();

        if (player.HorizontalInput == 0)
        {
            player.ChangeState(player.IdleState);
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
        player.Rb.linearVelocity = new Vector2(player.HorizontalInput * player.MoveSpeed, player.Rb.linearVelocityY);
    }

    public override void Exit()
    {
        player.Anim.SetFloat("xVelocity", 0f);
    }
}
