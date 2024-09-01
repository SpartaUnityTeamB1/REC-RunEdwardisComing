using UnityEngine;

public class EdwardIdleState : EdwardBaseState
{
    public EdwardIdleState(EdwardStateMachine stateMachine) : base(stateMachine)
    {
    
    }

    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Edward.AnimationData.IdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Edward.AnimationData.IdleParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.zero != stateMachine.MovementInput)
        {
            stateMachine.ChangeState(stateMachine.RunState);
            return;
        }
    }
}