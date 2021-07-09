using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TelePorting : MonoBehaviour
{
    public GameObject pointer; // 텔레포트 포인터가 될 오브젝트 선언

    public SteamVR_Action_Boolean teleport; // 텔레포트 기능과 연결할 컨트롤러 버튼 Action

    private SteamVR_Behaviour_Pose myPose = null; // 해당 컨트롤러의 동작 자세를 저장하는 변수

    private Transform myTransform = null;

    private bool hasPosition = false; // 포인터의 위치가 저장되었는지 확인하는 변수
    private bool isTeleporting = false; // 텔레포트 유무를 판단하여 텔레포트가 중복되지 않도록 하는 변수

    public float fadeTime = 0.5f; // 텔레포트할 때 fade되는 시간

    public Transform cameraRig;
    public Transform headPosition; // CameraRig 하위계층의 Camera

    // Raycast
    private Ray ray;
    private RaycastHit hitInfo;
    public float rayDistance = 40f; // Raycast가 가능한 최대 거리
    public LayerMask teleportLayer; // 텔레포트가 가능한 Layer 지정, Plane의 layer로서 Plane구역에서 텔레포트 가능

    void Start()
    {
        myPose = GetComponent<SteamVR_Behaviour_Pose>();
        myTransform = GetComponent<Transform>();
    }

    void Update()
    {
        // 텔레포트 버튼을 누르고 있는 중이면 텔레포트 함수를 호출
        if (teleport.GetState(myPose.inputSource))
        {
            // 포인터의 위치를 갱신
            hasPosition = UpdatePointer();
        }

        if (teleport.GetStateUp(myPose.inputSource))
        {
            // 텔레포트 함수 호출
            TelePort();
            pointer.SetActive(false);
        }
    }

    private bool UpdatePointer()
    {
        // 컨트롤러의 앞 방향으로 레이캐스트 발사
        ray = new Ray(myTransform.position, myTransform.forward);

        // 레이캐스트가 오브젝트에 닿으면 포인터의 위치를 변경 후 True를 반환하여 포인터를 활성화
        // layer를 함께 던져 해당 layer를 가진 오브젝트에 닿을 때만 포인터가 활성화되도록
        if (Physics.Raycast(ray, out hitInfo, rayDistance, teleportLayer))
        {
            pointer.transform.position = hitInfo.point;
            pointer.SetActive(true);
            return true;
        }
        
        //레이캐스트가 텔레포트 가능한 위치에 닿지 않으면 포인터의 위치를 변경하지 않고 포인터 비활성화
        pointer.SetActive(false);
        return false;
    }

    // 포인터의 위치를 받아 플레이어의 위치를 이동시키는 함수
    private void TelePort()
    {
        // 이동가능한 위치가 없거나 텔레포트중일 경우 텔레포트 호출을 무시
        if (hasPosition == false || isTeleporting == true) return;

        // 현재 플레이어의 위치 값을 저장
        Vector3 groundPosition 
            = new Vector3(headPosition.position.x, cameraRig.position.y, headPosition.position.z);

        // 포인터의 위치와 플레이어의 현재 위치 사이의 거리를 구함
        Vector3 translateVector = pointer.transform.position - groundPosition;

        StartCoroutine(MoveRig(cameraRig, translateVector));
    }

    private IEnumerator MoveRig(Transform cameraRig, Vector3 translation)
    {
        isTeleporting = true; // 텔레포트 중임을 알리는 변수

        SteamVR_Fade.Start(Color.black, fadeTime, true); // fade로 fadeTime동안 화면을 어둡게 가림

        // 이 코드 없으면 화면 어두워지지않고 바로바로 텔레포트 가능
        yield return new WaitForSeconds(fadeTime + 0.1f); // fadeTime + 0.1f동안 대기(텔레포트 불가)

        cameraRig.position += translation; // 텔레포트 실행(위치이동)
        SteamVR_Fade.Start(Color.clear, fadeTime, true); // 다시 화면을 밝게. Color.white 아님
        yield return new WaitForSeconds(fadeTime); // 텔레포트 딜레이
        isTeleporting = false; // 텔레포트 종료를 알리는 변수

        yield return null;
    }
}
