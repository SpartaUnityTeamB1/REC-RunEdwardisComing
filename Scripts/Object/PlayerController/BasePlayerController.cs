using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BasePlayerController : MonoBehaviourPun
{
    public Vector2 CurMovementInput { get; private set; }
    public Vector2 MouseDelta { get; private set; }

    protected virtual void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            CurMovementInput = context.ReadValue<Vector2>();
        else if (context.phase == InputActionPhase.Canceled)
            CurMovementInput = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        MouseDelta = context.ReadValue<Vector2>();
    }
}