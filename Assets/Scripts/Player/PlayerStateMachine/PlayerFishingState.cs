using UnityEngine;

public class PlayerFishingState : PlayerState
{
    private enum Phase { Idle, Prep, Casting, Wait, Catching }
    private Phase phase;
    private Phase previousPhase;
    private bool minigameActive;

    public PlayerFishingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.Anim.enabled = false;
        player.Sr.sprite = player.FishingIdleSprite;
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocityY);
        phase = Phase.Idle;
        previousPhase = Phase.Idle;
        minigameActive = false;
    }

    public override void Update()
    {
        // Track phase transitions and pause/resume accordingly
        if (phase != previousPhase)
        {
            bool wasMinigameActive = IsMinigamePhase(previousPhase);
            bool isMinigameActive = IsMinigamePhase(phase);

            // Pause if transitioning INTO a minigame phase
            if (!wasMinigameActive && isMinigameActive)
            {
                TimeOfDayManager.Instance?.PauseDay();
                minigameActive = true;
            }
            // Resume if transitioning OUT of a minigame phase
            else if (wasMinigameActive && !isMinigameActive)
            {
                TimeOfDayManager.Instance?.ResumeDay();
                minigameActive = false;
            }

            previousPhase = phase;
        }

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
                    player.Anim.Play("CastingAnimation", 0, 0f);
                    player.Anim.Update(0f);
                    phase = Phase.Casting;
                }
                break;

            case Phase.Casting:
                AnimatorStateInfo info = player.Anim.GetCurrentAnimatorStateInfo(0);
                bool inCastingClip = info.IsName("CastingAnimation");
                bool finished = inCastingClip && info.normalizedTime >= 1f;

                if (finished)
                {
                    AudioManager.Instance?.PlayBobberLand();
                    phase = Phase.Wait;
                    HookManager.Instance.StartWaiting();
                }
                break;

            case Phase.Wait:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (HookManager.Instance.TryHook())
                    {
                        phase = Phase.Catching;
                        player.Anim.enabled = false;
                    }
                    else
                    {
                        // Clicked too early — reel in and go back to fishing idle
                        HookManager.Instance.StopWaiting();
                        player.Anim.enabled = false;
                        player.Sr.sprite = player.FishingIdleSprite;
                        phase = Phase.Idle;
                    }
                }
                break;

            case Phase.Catching:
                break;
        }
    }

    private bool IsMinigamePhase(Phase p)
    {
        return p == Phase.Prep || p == Phase.Casting || p == Phase.Catching;
    }

    public override void FixedUpdate()
    {
        player.Rb.linearVelocity = new Vector2(0f, player.Rb.linearVelocityY);
    }

    public override void Exit()
    {
        // Resume if currently in a minigame phase
        if (minigameActive)
            TimeOfDayManager.Instance?.ResumeDay();

        player.Anim.enabled = true;
        player.Anim.ResetTrigger("Casting");
        player.Anim.Play("Blend Tree");
        HookManager.Instance.StopWaiting();
        HookManager.Instance.StopMinigame();
    }


    // getter for state
    public bool isWaiting() => phase == Phase.Wait || phase == Phase.Catching;
}
