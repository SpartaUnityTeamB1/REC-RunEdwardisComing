using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkPool
{
    public string tag;
    public int size;
}

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
}

public class ObjectPool : MonoBehaviourPunCallbacks
{
    [field: SerializeField] private List<Pool> pools = new List<Pool>();
    [field: SerializeField] private List<NetworkPool> networkPools = new List<NetworkPool>();

    private Dictionary<string, Queue<GameObject>> networkPoolDictionary;
    private Dictionary<string, int> networkPoolIndexDictionary;

    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, int> poolIndexDictionary;

    private GameObject prefab;
    private GameObject obj;

    private void Awake()
    {
        InitPool();
    }

    public void InitializeNetwork()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        InitNetworkPool();
    }

    private void InitNetworkPool()
    {
        networkPoolDictionary = new Dictionary<string, Queue<GameObject>>();
        networkPoolIndexDictionary = new Dictionary<string, int>();

        for (int i = 0; i < networkPools.Count; ++i)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            //리소스 매니저로 바꿀 예정
            prefab = Resources.Load<GameObject>(networkPools[i].tag);

            for (int j = 0; j < networkPools[i].size; ++j)
            {
                obj = PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);
                obj.transform.parent = this.transform;

                obj.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, false);

                queue.Enqueue(obj);
            }

            networkPoolDictionary.Add(networkPools[i].tag, queue);
            networkPoolIndexDictionary.Add(networkPools[i].tag, i);
        }
    }

    private void InitPool()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolIndexDictionary = new Dictionary<string, int>();

        for (int i = 0; i < pools.Count; ++i)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int j = 0; j < pools[i].size; ++j)
            {
                obj = Instantiate(pools[i].prefab, this.transform);

                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            poolDictionary.Add(pools[i].tag, queue);
            poolIndexDictionary.Add(pools[i].tag, i);
        }
    }

    public void Clear()
    {
        if (null != poolDictionary)
        {
            foreach (var objList in poolDictionary)
            {
                foreach (var obj in objList.Value)
                    obj.SetActive(false);
            }
        }
        
        if (null != networkPoolDictionary)
        {
            foreach (var objList in networkPoolDictionary)
            {
                foreach (var obj in objList.Value)
                    obj.SetActive(false);
            }
        }
    }

    public GameObject SpawnFromNetworkPool(string tag)
    {
        if (!networkPoolDictionary.ContainsKey(tag))
            return null;

        obj = networkPoolDictionary[tag].Peek().gameObject;

        if (!obj.activeInHierarchy)
        {
            obj = networkPoolDictionary[tag].Dequeue();
            obj.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, true);
        }
        else
        {
            prefab = Resources.Load<GameObject>(tag);

            obj = PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);
            obj.transform.parent = this.transform;
        }

        networkPoolDictionary[tag].Enqueue(obj);

        return obj;
    }

    public GameObject SpawnFromPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
            return null;

        obj = poolDictionary[tag].Peek().gameObject;

        if (!obj.activeInHierarchy)
        {
            obj = poolDictionary[tag].Dequeue();
            obj.SetActive(true);
        }
        else
            obj = Instantiate(pools[poolIndexDictionary[tag]].prefab, this.transform);

        poolDictionary[tag].Enqueue(obj);

        return obj;
    }

    public void ReleaseNetworkObjects()
    {
        if (null != networkPoolDictionary)
        {
            foreach (var objList in networkPoolDictionary)
                objList.Value.Clear();

            networkPoolDictionary.Clear();

            networkPoolDictionary = null;
        }
    }
}