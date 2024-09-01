using Photon.Pun;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    [field: SerializeField] public SceneIndex nextSceneIndex { get; set; }
    [field: SerializeField] public SceneIndex prevSceneIndex { get; set; }

    protected virtual void Start()
    {
        GameManager.Instance.SceneChange(this);
    }
}