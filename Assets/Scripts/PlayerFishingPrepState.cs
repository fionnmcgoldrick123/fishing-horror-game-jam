using UnityEngine;

public class PlayerFishingPrepState : PlayerState
{
    public PlayerFishingPrepState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.Anim.SetBool("isPrepping", true);
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.ExitFishing();
            return;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            player.ChangeState(player.FishingCastState);
            return;
        }
    }

    public override void Exit()
    {
        player.Anim.SetBool("isPrepping", false);
    }
}
