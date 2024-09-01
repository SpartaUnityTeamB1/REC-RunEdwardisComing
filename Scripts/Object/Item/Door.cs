
using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : BaseObject
{
    [SerializeField] private float increase;
    [SerializeField] private List<Transform> doorTransform = new List<Transform>();
    [SerializeField] private float goal;
    [SerializeField] private float goalTime;
    [SerializeField] private KeyIndex index;

    private float curOpening = 0f;
    private Coroutine doorCor;
    private bool isOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (IsLayerMatched(targetLayer.value, other.gameObject.layer) && other.gameObject.GetPhotonView().IsMine && !isOpen)
        {
            GameManager.Instance.ShowUI<PopupLootingText>(UI.Popup);

            GameManager.Instance.PlayerDoll.PlayerController.InteractionAction += OpenDoor;
            GameManager.Instance.PlayerDoll.PlayerController.CancelinteractionAction += CancelOpen;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsLayerMatched(targetLayer.value, other.gameObject.layer) && other.gameObject.GetPhotonView().IsMine)
        {
            CancelOpen();

            GameManager.Instance.HideUI<PopupLootingText>();

            GameManager.Instance.PlayerDoll.PlayerController.InteractionAction -= OpenDoor;
            GameManager.Instance.PlayerDoll.PlayerController.CancelinteractionAction -= CancelOpen;
        }
    }

    private void OpenDoor()
    {
        // 마스터한테 열어달라는 RPC 보내야할 듯 <- 일단 보류
        if ((null == doorCor) && GameManager.Instance.CheckOpenDoor(index))
        {
            GameManager.Instance.PlayerDoll.OpenDoor();
            
            doorCor = StartCoroutine(IncreaseOpen());

            // SceneUI로 문 열린거 보여주는 게이지 ui 보여주기
        }
    }

    private void CancelOpen()
    {
        if (null != doorCor)
        {
            StopCoroutine(doorCor);
            doorCor = null;
        }

        GameManager.Instance.PlayerDoll.CancelOpenDoor();
    }

    private IEnumerator IncreaseOpen()
    {
        while (1f > curOpening)
        {
            photonView.RPC("Opening", RpcTarget.All);

            yield return null;
        }

        isOpen = true;

        CancelOpen();

        GameManager.Instance.HideUI<PopupLootingText>();

        GameManager.Instance.PlayerDoll.PlayerController.InteractionAction -= OpenDoor;
        GameManager.Instance.PlayerDoll.PlayerController.CancelinteractionAction -= CancelOpen;

        photonView.RPC("Open", RpcTarget.All);

        doorCor = null;
    }

    [PunRPC]
    private void Opening()
    {
        curOpening += increase;
    }

    [PunRPC]
    private void Open()
    {
        switch (index)
        {
            case KeyIndex.FirstKey:
                doorTransform[0].DOLocalMoveZ(goal, goalTime);
                break;
            case KeyIndex.SecondKey:
                doorTransform[0].DOLocalRotate(Vector3.up * goal, goalTime);
                break;
            case KeyIndex.EscapeKey:
                doorTransform[0].DOLocalRotate(Vector3.up * goal, goalTime);
                doorTransform[1].DOLocalRotate(Vector3.up * -goal, goalTime);
                break;
        }
    }
}