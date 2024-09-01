using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Doll.AnimationData.IdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Doll.AnimationData.IdleParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if(Vector2.zero != stateMachine.MovementInput && !GameManager.Instance.PlayerDoll.IsCrazy)
        {
            stateMachine.ChangeState(stateMachine.MoveState);
            return;
        }
        else if (GameManager.Instance.PlayerDoll.IsCrazy)
        {
            stateMachine.ChangeState(stateMachine.CrazyState);
        }
    }
}