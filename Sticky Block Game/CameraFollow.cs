using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;
    [SerializeField] float smoothSpeed;
    [SerializeField] Vector3 offset;

    public Transform target;

    void LateUpdate()
    {
        if (playerManager.levelState == PlayerManager.LevelState.NotFinished)
        {
            Vector3 desiredPos = target.position + offset;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
            transform.position = new Vector3(transform.position.x, transform.position.y, smoothedPos.z);
        }
    }
}
