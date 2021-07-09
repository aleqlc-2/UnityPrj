using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Collider))]
public class ControllerCollider : MonoBehaviour
{
    public SteamVR_TrackedObject controller;

    void Update()
    {
        if (controller != null)
        {
            transform.position = controller.transform.position;
            transform.rotation = controller.transform.rotation;
        }
    }
}
