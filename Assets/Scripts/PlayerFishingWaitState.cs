using UnityEngine;

public class PlayerFishingWaitState : PlayerState
{
    public PlayerFishingWaitState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // CastingAnimation has no outgoing transitions, force back to fishing pose
        player.Anim.Play("FishingAnimation");
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.ExitFishing();
            return;
        }

        // Click to reel in and return to fishing idle stance
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            player.ChangeState(player.FishingIdleState);
            return;
        }
    }
}
