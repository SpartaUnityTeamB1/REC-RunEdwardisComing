using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOpenDoorState : PlayerBaseState
{
    public PlayerOpenDoorState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Doll.AnimationData.OpenDoorParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Doll.AnimationData.OpenDoorParameterHash);
    }

    public override void PhysicsUpdate()
    {

    }
}
