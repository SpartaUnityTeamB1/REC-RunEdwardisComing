using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEscapeState : PlayerBaseState
{
    public PlayerEscapeState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Doll.AnimationData.EscapeParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Doll.AnimationData.EscapeParameterHash);
    }

    public override void PhysicsUpdate()
    {

    }
}
