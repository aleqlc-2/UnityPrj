using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerManager : MonoBehaviour
{
    public static ControllerManager Instance;

    public SteamVR_TrackedObject rightController;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Destroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
