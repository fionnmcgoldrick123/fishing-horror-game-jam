using UnityEngine;

public class PlayerFishingIdleState : PlayerState
{
    public PlayerFishingIdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.Anim.SetBool("isFishing", true);
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.ExitFishing();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            player.ChangeState(player.FishingPrepState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocityY);
    }
}
