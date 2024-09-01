using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Doll.AnimationData.DeadParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        
        StopAnimation(stateMachine.Doll.AnimationData.DeadParameterHash);
    }

    public override void PhysicsUpdate()
    {

    }
}
