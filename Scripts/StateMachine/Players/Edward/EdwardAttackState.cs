using UnityEngine;

public class EdwardAttackState : EdwardBaseState
{
    private AnimatorStateInfo animStateInfo;
    private Edward edward; 

    public EdwardAttackState(EdwardStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        edward = stateMachine.Edward;

        GameManager.Instance.HitChange(true);

        StartAnimation(stateMachine.Edward.AnimationData.AttackParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Edward.AnimationData.AttackParameterHash);

        stateMachine.Edward.CantAttack();

        GameManager.Instance.HitChange(false);
    }

    public override void Update()
    {
        base.Update();

        animStateInfo = edward.Animator.GetCurrentAnimatorStateInfo(0);

        if (animStateInfo.IsName("Attack") && (edward.AnimationData.AttackExitTime < animStateInfo.normalizedTime))
        {
            if (Vector2.zero != stateMachine.MovementInput)
                stateMachine.ChangeState(stateMachine.RunState);
            else
                stateMachine.ChangeState(stateMachine.IdleState);

            return;
        }
    }
}