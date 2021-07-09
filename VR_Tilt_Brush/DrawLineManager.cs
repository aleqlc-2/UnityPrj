using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DrawLineManager : MonoBehaviour
{
    public SteamVR_Action_Boolean trigger;
    public SteamVR_TrackedObject trackedObject;
    
    private LineRenderer currLine;

    private int numClicks = 0;

    public Material lMat;

    void Update()
    {
        if (trigger.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            GameObject go = new GameObject();
            currLine = go.AddComponent<LineRenderer>();
            currLine.startWidth = .01f;
            currLine.endWidth = .01f;
            currLine.material = lMat;
            numClicks = 0;
        }
        else if (trigger.GetState(SteamVR_Input_Sources.RightHand))
        {
            currLine.positionCount = numClicks + 1;
            currLine.SetPosition(numClicks, trackedObject.transform.position);
            numClicks++;
        }
    }
}
