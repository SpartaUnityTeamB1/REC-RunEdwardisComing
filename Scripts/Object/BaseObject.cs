using Photon.Pun;
using UnityEngine;

public class BaseObject : MonoBehaviourPun
{
    [SerializeField] protected LayerMask targetLayer;

    public bool IsLayerMatched(int value, int layer)
    {
        return value == (value | 1 << layer);
    }

    [PunRPC]
    public void RPCSetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}