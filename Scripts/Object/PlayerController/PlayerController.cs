using Cinemachine;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BasePlayerController
{
    public Action InteractionAction;
    public Action CancelinteractionAction;

    private int cursor = 0;
    private BaseCharacter player;

    protected override void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        base.FixedUpdate();
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            InteractionAction?.Invoke();
        }
        if (InputActionPhase.Canceled == context.phase)
        {
            CancelinteractionAction?.Invoke();
        }
    }

    public void OnCameraChange(InputAction.CallbackContext context) 
    { 
        if(context.phase == InputActionPhase.Started && GameManager.Instance.PlayerDoll.IsObserve && !GameManager.Instance.isGameOver)
        {
            cursor++;

            if (cursor == GameManager.Instance.PlayerList.Count)
                cursor = 0;

            player = GameManager.Instance.PlayerList[cursor];
            GameManager.Instance.PlayerDoll.cineMachine.Follow = player.LookPosition.transform;
            GameManager.Instance.PlayerDoll.cineMachine.LookAt = player.LookPosition.transform;
                
        }
    }

}