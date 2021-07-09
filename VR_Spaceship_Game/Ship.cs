using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Ship : MonoBehaviour
{
    public ParticleSystem laser;

    private SteamVR_TrackedObject controller; // private

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward))
        {
            // 발사
            laser.Emit(1); // assumes laser is already player
        }

        // 핸들이 움직이는대로 비행기 위치 변경
        if (controller != null)
        {
            transform.position = controller.transform.position;
            transform.rotation = controller.transform.rotation;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        ControllerCollider collider = other.GetComponent<ControllerCollider>();
        if (collider != null)
        {
            controller = collider.controller;
        }
    }
}
