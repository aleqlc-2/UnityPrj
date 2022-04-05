using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform myTransform;
    private Transform target;

    public Vector3 offset = new Vector3(-8f, 8.6f, 0f);

    void Start()
    {
        target = GameObject.FindGameObjectWithTag(TagManager.PLYAER_TAG).transform;

        myTransform = transform;
    }

    void LateUpdate()
    {
        if (target)
        {
            myTransform.position = target.position + offset;
        }
    }
}
