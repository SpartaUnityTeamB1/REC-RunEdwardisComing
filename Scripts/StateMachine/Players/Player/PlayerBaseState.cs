using UnityEngine;

public class PlayerBaseState : IState
{
    protected PlayerStateMachine stateMachine;

    private Vector3 moveDir;

    public PlayerBaseState(PlayerStateMachine stateMachine)
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
        stateMachine.MovementInput = stateMachine.Doll.PlayerController.CurMovementInput;
    }

    public virtual void PhysicsUpdate()
    {
        Move();
        Rotate();
    }

    public virtual void Update()
    {
        
    }

    protected void StartAnimation(int animatorHash)
    {
        stateMachine.Doll.Animator.SetBool(animatorHash, true);
    }

    protected void StopAnimation(int animatorHash)
    {
        stateMachine.Doll.Animator.SetBool(animatorHash, false);
    }

    private void Move()
    {
        Vector3 lookFoward = new Vector3(stateMachine.Doll.Cam.gameObject.transform.forward.x, 0f, stateMachine.Doll.Cam.gameObject.transform.forward.z).normalized;
        Vector3 lookRight = new Vector3(stateMachine.Doll.Cam.gameObject.transform.right.x, 0f, stateMachine.Doll.Cam.gameObject.transform.right.z).normalized;
        moveDir = lookFoward * stateMachine.MovementInput.y + lookRight * stateMachine.MovementInput.x;
        moveDir *= (stateMachine.Doll.Stat.speed * Time.fixedDeltaTime);

        stateMachine.Doll.transform.position += moveDir;
    }

    public void Rotate()
    {
        if (moveDir == Vector3.zero)
            return;

        Quaternion viewRot = Quaternion.LookRotation(moveDir);
        stateMachine.Doll.transform.rotation = Quaternion.Lerp(stateMachine.Doll.transform.rotation, viewRot, 0.3f);

    }
}
