using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Doll.AnimationData.MoveParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Doll.AnimationData.MoveParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if(Vector2.zero == stateMachine.MovementInput && !GameManager.Instance.PlayerDoll.IsCrazy)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
        else if (GameManager.Instance.PlayerDoll.IsCrazy)
        {
            stateMachine.ChangeState(stateMachine.CrazyState);
        }
    }
}