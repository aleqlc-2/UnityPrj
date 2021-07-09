using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Sniper : MonoBehaviour
{
    public SteamVR_Action_Boolean trigger;
    public SteamVR_Action_Boolean touchpad;

    public SteamVR_TrackedObject rightController;    

    public GameObject BulletPrefab;

    public Transform BulletSpawnPoint;

    public AudioSource audio;

    private float speed = 250f;

    // public const float minFOV = 10f;
    // public const float maxFOV = 90f;
    // public Camera scopeCamera;

    void Update()
    {
        if (trigger.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            GameObject go =
             Instantiate(BulletPrefab, BulletSpawnPoint.position, BulletSpawnPoint.transform.rotation) as GameObject;
            
            go.transform.Rotate(90f, 0f, 0f);

            // Vector3 * 스피드
            go.GetComponent<Rigidbody>().velocity = BulletSpawnPoint.transform.forward * speed;

            audio.Play();
        }

        // 터치패드 누르면 zoom 발동되도록
        // if (touchpad.GetStateDown(SteamVR_Input_Sources.RightHand))
        // {
        //     float touchY = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y; // deprecated
        //     float fov = scopeCamera.fieldOfView - 1f * touchY;
        //     if(fov < minFOV)
        //         scopeCamera.fieldOfView = minFOV;
        //     else if(fov > maxFOV)
        //         scopeCamera.fieldOfView = maxFOV;
        //     else
        //         scopeCamera.fieldOfView = fov;
        // }
    }
}
