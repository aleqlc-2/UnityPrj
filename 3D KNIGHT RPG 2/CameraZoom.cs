using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoom_Sensitivity = 15f;
    public float zoom_Speed = 20f;
    public float zoom_Min = 30f;
    public float zoom_Max = 70f;

    private float z;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        z = mainCamera.fieldOfView;
    }

    void Update()
    {
        z -= Input.GetAxis("Mouse ScrollWheel") * zoom_Sensitivity;
        z = Mathf.Clamp(z, zoom_Min, zoom_Max);
    }

    void LateUpdate()
    {
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, z, Time.deltaTime * zoom_Speed);
    }
}
