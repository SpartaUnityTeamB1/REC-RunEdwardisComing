using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class EdwardController : BasePlayerController
{
    public bool IsAttacking { get; private set; } = false;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (InputActionPhase.Started == context.phase)
            IsAttacking = true;
        else if (InputActionPhase.Canceled == context.phase)
            IsAttacking = false;
    }
}