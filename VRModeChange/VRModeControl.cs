using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRModeControl : MonoBehaviour
{
    void Update()
    {
        // ESC 누르면 일반모드로 전환(폰에서는 뒤로가기버튼)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(LoadDevice("None"));
        }
    }

    // 버튼누르면 VR모드로 전환
    public void StereoView() // 인스펙터창에서 버튼에 바인딩
    {
        StartCoroutine(LoadDevice("Cardboard"));
    }

    IEnumerator LoadDevice(string newDevice)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null; // 1프레임 쉬고
        XRSettings.enabled = true;
    }
}
