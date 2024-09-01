using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerCrazyState : PlayerBaseState
{
    public PlayerCrazyState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Doll.AnimationData.CrazyParameterHash);
    }

    public override void Exit()
    { 
        base.Exit();

        StopAnimation(stateMachine.Doll.AnimationData.CrazyParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (!GameManager.Instance.PlayerDoll.IsCrazy && Vector2.zero != stateMachine.MovementInput)
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }
        else if(!GameManager.Instance.PlayerDoll.IsCrazy && Vector2.zero == stateMachine.MovementInput)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}
