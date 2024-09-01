using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillSpawner : BaseObject
{
    [SerializeField]
    private List<GameObject> spawnList;

    [SerializeField] private string pillTag;

    private GameObject pillObject;

    private int random;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            SpawnPill();
    }

    public void SpawnPill()
    {
        ObjectPool objPool = GameManager.Instance.ObjectPool;

        for (int i = 0; i < spawnList.Count; i++)
        {
            random = Random.Range(0, 2);

            if (random == 0)
            {
                pillObject = objPool.SpawnFromNetworkPool(pillTag);

                pillObject.transform.position = spawnList[i].transform.position;
            }
        }
    }
}