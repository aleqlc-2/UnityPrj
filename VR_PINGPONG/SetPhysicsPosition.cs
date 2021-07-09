using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SetPhysicsPosition : MonoBehaviour
{
    private Rigidbody r;

    public SteamVR_TrackedObject controller;

    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        r.MovePosition(controller.transform.position);
        r.MoveRotation(controller.transform.rotation);
    }
}
