using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRotateCamera : MonoBehaviour
{
    
    void Start()
    {
        StartCoroutine(carRotate());
    }

    IEnumerator carRotate()
    {
        yield return new WaitForSeconds(3.5f);
        this.gameObject.SetActive(false); // this 빼도됨
    }
}
