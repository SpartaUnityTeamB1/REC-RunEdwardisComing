using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : BaseObject
{
    private List<Vector3> positionList;
    private List<bool> isSpawnList = new List<bool>();

    private string objectTag;
    private GameObject obj;
    private int count;
    private int random;

    public void Spawn(Vector3 position, string tag)
    {
        obj = GameManager.Instance.ObjectPool.SpawnFromNetworkPool(tag);

        obj.transform.position = position;
    }

    public void Spawn(SpawnData spawnData)
    {
        positionList = spawnData.positions;

        objectTag = spawnData.tag;

        count = spawnData.maxCount;

        isSpawnList.Clear();
        foreach (var position in positionList)
            isSpawnList.Add(false);

        for (int i = 0; i < count;)
        {
            random = Random.Range(0, positionList.Count);

            if (!isSpawnList[random])
            {
                obj = GameManager.Instance.ObjectPool.SpawnFromNetworkPool(objectTag);

                obj.transform.position = positionList[random];

                isSpawnList[random] = true;

                ++i;
            }
        }
    }
}