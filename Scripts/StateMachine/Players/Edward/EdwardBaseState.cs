using Photon.Pun;
using UnityEngine;

public class EdwardBaseState : IState
{
    protected EdwardStateMachine stateMachine;

    private Vector3 dir;

    private Vector3 camAngles = Vector3.zero;
    private Vector3 edwardAngles = Vector3.zero;

    public EdwardBaseState(EdwardStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
        
    }

    public virtual void HandleInput()
    {
        stateMachine.MovementInput = stateMachine.Edward.PlayerController.CurMovementInput;
        stateMachine.RotateInput = stateMachine.Edward.PlayerController.MouseDelta;

        if (stateMachine.Edward.PlayerController is EdwardController)
        {
            stateMachine.AttackInput = ((EdwardController)stateMachine.Edward.PlayerController).IsAttacking;
        }
    }

    public virtual void PhysicsUpdate()
    {
        Move();
        Rotate();
    }

    public virtual void Update()
    {
        if (stateMachine.Edward.CanAttack && stateMachine.AttackInput)
        {
            stateMachine.ChangeState(stateMachine.AttackState);
            return;
        }
    }

    protected void StartAnimation(int animatorHash)
    {
        stateMachine.Edward.Animator.SetBool(animatorHash, true);
        //stateMachine.Edward.AnimationStart(animatorHash);
    }

    protected void StopAnimation(int animatorHash)
    {
        stateMachine.Edward.Animator.SetBool(animatorHash, false);
        //stateMachine.Edward.AnimationStop(animatorHash);
    }

    private void Move()
    {
        dir = ((stateMachine.Edward.transform.forward * stateMachine.MovementInput.y) +
            (stateMachine.Edward.transform.right * stateMachine.MovementInput.x)).normalized;

        dir *= (stateMachine.Edward.Stat.speed * Time.fixedDeltaTime);

        stateMachine.Edward.transform.position += dir;
    }

    private void Rotate()
    {
        stateMachine.CamCurXRot += (stateMachine.RotateInput.y * stateMachine.Edward.Stat.lookSensitivity);
        stateMachine.CamCurXRot = Mathf.Clamp(stateMachine.CamCurXRot, stateMachine.Edward.MinXLook, stateMachine.Edward.MaxXLook);

        camAngles.x = -stateMachine.CamCurXRot;

        stateMachine.Edward.CameraPos.localEulerAngles = camAngles;

        edwardAngles.y = stateMachine.RotateInput.x * stateMachine.Edward.Stat.lookSensitivity;
        stateMachine.Edward.transform.eulerAngles += edwardAngles;
    }
}