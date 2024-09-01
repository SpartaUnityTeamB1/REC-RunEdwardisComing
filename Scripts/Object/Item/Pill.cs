using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pill : BaseObject
{
    [SerializeField] private AudioClip getSFX;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsLayerMatched(targetLayer.value, other.gameObject.layer) && other.gameObject.GetPhotonView().IsMine)
        {
            GameManager.Instance.ShowUI<PopupLootingText>(UI.Popup);

            GameManager.Instance.PlayerDoll.PlayerController.InteractionAction += UsePill;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsLayerMatched(targetLayer.value, other.gameObject.layer) && other.gameObject.GetPhotonView().IsMine)
        {
            GameManager.Instance.HideUI<PopupLootingText>();

            GameManager.Instance.PlayerDoll.PlayerController.InteractionAction -= UsePill;
        }
    }

    private void UsePill()
    {
        GameManager.Instance.PlayerDoll.IsCrazy = false;
        GameManager.Instance.PlayerDoll.HealStress();

        photonView.RPC("PlaySFXRPC", RpcTarget.All);

        //GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, false);
        //GameManager.Instance.PlaySFX(getSFX);

        GameManager.Instance.HideUI<PopupLootingText>();
        GameManager.Instance.PlayerDoll.PlayerController.InteractionAction -= UsePill;
    }

    [PunRPC]
    private void PlaySFXRPC()
    {
        StartCoroutine(PlaySFX());
    }

    private IEnumerator PlaySFX()
    {
        source.PlayOneShot(getSFX);

        while (source.isPlaying)
            yield return null;

        photonView.RPC("RPCSetActive", RpcTarget.All, false);
    }
}
