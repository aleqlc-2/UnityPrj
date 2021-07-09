using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HadokenHand : MonoBehaviour
{
    public SteamVR_Action_Boolean trigger;
    
    public HadokenBehavior hadokenPrefab; // hadoken의 동작정의를 위해 자료형을 HadokenBehavior로

    [HideInInspector]
    public HadokenBehavior spawnedHadoken;  // hadoken의 동작정의를 위해 자료형을 HadokenBehavior로

    // 다른 클래스에서 HadokenHand 스크립트객체를 생성하여 handPos호출하면
    // 이 스크립트가 부착된 오브젝트의 transform.position를 받을 수 있음
    // public Vector3 handPos
    // {
    //     get
    //     {
    //         return transform.position;
    //     }
    // }

    public void StartHadoken()
    {
        spawnedHadoken = Instantiate(hadokenPrefab, transform.position, transform.rotation);

        // 현재 움직이는 핸들의 Transform 컴포넌트 값들을 모두 HadokenBehavior클래스의 handle변수로 전달하면
        // HadokenBehavior클래스에서 lerp를 이용하여 hadoken이 handle의 위치로 따라가도록
        // 이 코드 없으면 hadoken이 안따라감
        spawnedHadoken.handle = transform;
    }

    public void ThrowHadoken(SteamVR_Behaviour_Pose myHand)
    {
        spawnedHadoken.Throw(myHand);
    }
}
