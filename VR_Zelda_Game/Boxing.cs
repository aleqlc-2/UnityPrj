using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Boxing : MonoBehaviour
{
    public SteamVR_TrackedObject hand;
    private Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        r.MovePosition(hand.transform.position);
        r.MoveRotation(hand.transform.rotation);
    }
}
