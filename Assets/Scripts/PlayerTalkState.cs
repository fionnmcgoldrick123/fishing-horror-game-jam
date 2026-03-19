using UnityEngine;

public class PlayerTalkState : PlayerState
{
    public PlayerTalkState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.Anim.SetFloat("xVelocity", 0f);
    }

    public override void FixedUpdate()
    {
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocityY);
    }
}
