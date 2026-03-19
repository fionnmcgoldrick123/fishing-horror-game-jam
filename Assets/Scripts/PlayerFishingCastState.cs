using UnityEngine;

public class PlayerFishingCastState : PlayerState
{
    public PlayerFishingCastState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.Anim.SetTrigger("Casting");
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.ExitFishing();
            return;
        }

        // Wait for CastingAnimation to finish, then move to waiting
        AnimatorStateInfo info = player.Anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("CastingAnimation") && info.normalizedTime >= 1f)
        {
            player.ChangeState(player.FishingWaitState);
        }
    }

    public override void Exit()
    {
        player.Anim.ResetTrigger("Casting");
    }
}
