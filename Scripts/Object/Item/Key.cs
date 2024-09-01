using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : BaseObject
{
    [SerializeField] private KeyIndex index;
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

            GameManager.Instance.PlayerDoll.PlayerController.InteractionAction += GetKey;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsLayerMatched(targetLayer.value, other.gameObject.layer) && other.gameObject.GetPhotonView().IsMine)
        {
            GameManager.Instance.HideUI<PopupLootingText>();

            GameManager.Instance.PlayerDoll.PlayerController.InteractionAction -= GetKey;
        }
    }

    private void GetKey()
    {
        GameManager.Instance.IncreaseKey((int)index);

        photonView.RPC("PlaySFXRPC", RpcTarget.All);

        GameManager.Instance.PlaySFX(getSFX);

        GameManager.Instance.HideUI<PopupLootingText>();
        GameManager.Instance.PlayerDoll.PlayerController.InteractionAction -= GetKey;
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
