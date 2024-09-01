using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {
        yield return new WaitForSeconds(2f);

        transform.DOLocalMoveZ(-1.8f, 2f);
    }
}
