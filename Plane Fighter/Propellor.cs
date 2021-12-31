using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propellor : MonoBehaviour
{
    private bool rotating = true;

    void Update()
    {
        if (rotating)
        {
            transform.Rotate(Vector3.right * 40); // 40 곱하면 더 빨리 회전함
        }
    }

    public void PauseRotation()
    {
        rotating = false;
    }

    public void ResumeRotation()
    {
        rotating = true;
    }
}
