using UnityEngine;

public class PlayerFishingState : PlayerState
{
    private enum Phase { Idle, Prep, Casting, Wait }
    private Phase phase;

    public PlayerFishingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.Anim.enabled = false;
        player.Sr.sprite = player.FishingIdleSprite;
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocityY);
        phase = Phase.Idle;
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.ExitFishing();
            return;
        }

        switch (phase)
        {
            case Phase.Idle:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    player.Sr.sprite = player.FishingPrepSprite;
                    phase = Phase.Prep;
                }
                break;

            case Phase.Prep:
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    player.Anim.enabled = true;
                    player.Anim.SetTrigger("Casting");
                    phase = Phase.Casting;
                }
                break;

            case Phase.Casting:
                AnimatorStateInfo info = player.Anim.GetCurrentAnimatorStateInfo(0);
                bool inCastingClip = info.IsName("CastingAnimation");
                bool finished = inCastingClip && info.normalizedTime >= 1f;

                if (finished)
                {
                    // player.Anim.enabled = false;
                    // player.Sr.sprite = player.FishingIdleSprite;
                    phase = Phase.Wait;
                }
                break;

            case Phase.Wait:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    //    if (fishBite)
                    //     {
                    //         // trigger reel input prompt
                    //     }


                    player.Anim.enabled = false;
                    player.Sr.sprite = player.FishingIdleSprite;
                    phase = Phase.Idle;
                }
                break;
        }
    }

    public override void FixedUpdate()
    {
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocityY);
    }

    public override void Exit()
    {
        player.Anim.enabled = true;
        player.Anim.ResetTrigger("Casting");
    }
}
