using UnityEngine;

public class EdwardRunState : EdwardBaseState
{
    public EdwardRunState(EdwardStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Edward.AnimationData.MoveParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Edward.AnimationData.MoveParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.zero == stateMachine.MovementInput)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }   
    }
}