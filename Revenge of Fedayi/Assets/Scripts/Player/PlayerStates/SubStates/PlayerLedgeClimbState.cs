using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    private Vector2 detectedPosition;
    private Vector2 cornerPosition;
    private Vector2 startPosition;
    private Vector2 stopPosition;

    private bool isHanging;
    private bool isClimbing;

    private int xInput;
    private int yInput;

    private string climbLedgeAnimName = "climbLedge";

    public PlayerLedgeClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        player.Animator.SetBool(climbLedgeAnimName, false);
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        isHanging = true;
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityZero();
        player.transform.position = detectedPosition;
        cornerPosition = player.DetermineCornerPosition();

        startPosition.Set(cornerPosition.x - (player.FacingDirection * playerData.startOffSet.x), cornerPosition.y - playerData.startOffSet.y);
        stopPosition.Set(cornerPosition.x + (player.FacingDirection * playerData.stopOffSet.x), cornerPosition.y + playerData.stopOffSet.y);

        player.transform.position = startPosition;
    }

    public override void Exit()
    {
        base.Exit();

        isHanging = false;

        if(isClimbing)
        {
            player.transform.position = stopPosition;
            isClimbing = false;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else
        {
            xInput = player.InputHandler.NormalizedInputX;
            yInput = player.InputHandler.NormalizedInputY;

            player.SetVelocityZero();
            player.transform.position = startPosition;

            if (xInput == player.FacingDirection && isHanging && !isClimbing)
            {
                isClimbing = true;
                player.Animator.SetBool(climbLedgeAnimName, true);
            }
            else if (yInput == -1 && isHanging && !isClimbing)
            {
                stateMachine.ChangeState(player.InAirState);
            }
        }        
    }

    public void SetDetectedPosition(Vector2 detectedPosition)
    {
        this.detectedPosition = detectedPosition;
    }
}
