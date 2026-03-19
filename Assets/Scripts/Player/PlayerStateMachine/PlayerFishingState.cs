using NUnit.Framework;
using UnityEngine;

public class PlayerFishingState : PlayerState
{
    private enum Phase { Idle, Prep, Casting, Wait, Catching }
    private Phase phase;

    public PlayerFishingState(PlayerController player) : base(player) { }


    public override void Enter()
    {
        player.Anim.enabled = false;
        player.Sr.sprite = player.FishingIdleSprite;
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocityY);
        phase = Phase.Idle;
        HookManager.Instance.OnCatchingStarted += HandleFishBite;
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
                    phase = Phase.Wait;
                }
                break;

            case Phase.Wait:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // player.Anim.enabled = false;
                    // player.Sr.sprite = player.FishingIdleSprite;
                    // phase = Phase.Idle;

                    phase = Phase.Catching;
                }
                break;

            case Phase.Catching:
                // Handle catching logic here (e.g., show skill check UI, determine success/failure)
                player.Anim.enabled = false;
                player.DisablePlayerMovement();
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
        HookManager.Instance.OnCatchingStarted -= HandleFishBite;
    }

    private void HandleFishBite() => phase = Phase.Catching;


    // getter for state
    public bool isWaiting() => phase == Phase.Wait || phase == Phase.Catching;
}
