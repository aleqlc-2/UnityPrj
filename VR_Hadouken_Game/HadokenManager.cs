using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HadokenManager : MonoBehaviour
{
    public HadokenHand[] hands;

    // 해당 컨트롤러의 동작 자세(pose)를 저장하는 변수, Input Source 개체
    private SteamVR_Behaviour_Pose myHand = null;

    void Start()
    {
        myHand = GetComponent<SteamVR_Behaviour_Pose>();
    }

    void Update()
    {
        foreach (HadokenHand h in hands)
        {
            if (h.gameObject == null) continue;

            if (h.gameObject.name == "Controller (left)")
            {
                if (h.trigger.GetStateDown(SteamVR_Input_Sources.LeftHand) && h.spawnedHadoken == null)
                {
                    print(h.gameObject.name + " pressed down");
                    h.StartHadoken();
                }
                else if (h.trigger.GetStateUp(myHand.inputSource) && h.spawnedHadoken != null)
                {
                    print(h.gameObject.name + " released");
                    h.ThrowHadoken(myHand);
                }
            }

            if (h.gameObject.name == "Controller (right)")
            {
                if (h.trigger.GetStateDown(SteamVR_Input_Sources.RightHand) && h.spawnedHadoken == null)
                {
                    print(h.gameObject.name + " pressed down");
                    h.StartHadoken();
                }
                else if (h.trigger.GetStateUp(myHand.inputSource) && h.spawnedHadoken != null)
                {
                    print(h.gameObject.name + " released");
                    h.ThrowHadoken(myHand);
                }
            }
        }
    }
}
