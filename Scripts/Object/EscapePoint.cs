using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePoint : BaseObject
{
    private void OnTriggerEnter(Collider other)
    {
        if (IsLayerMatched(targetLayer.value, other.gameObject.layer) && other.gameObject.GetPhotonView().IsMine)
        {
            GameManager.Instance.DollEscape();
            GameManager.Instance.PlayerDoll.stateMachine.ChangeState(GameManager.Instance.PlayerDoll.stateMachine.EscapeState);
            GameManager.Instance.PlayerList.RemoveAt(GameManager.Instance.PlayerDoll.index);
            GameManager.Instance.PlayerDoll.IsObserve = true;
        }
    }
}
