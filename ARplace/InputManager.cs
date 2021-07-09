using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera arCam;

    [SerializeField] private ARRaycastManager _raycastManager;

    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    private Touch touch;

    [SerializeField] private GameObject crosshair; // Marker를 할당

    private Pose pose;

    void Update()
    {
        CrosshairCalculation();

        touch = Input.GetTouch(0);

        // 터치된게 없으면 return
        if (Input.touchCount <= 0 || touch.phase != TouchPhase.Began) return;
        if (!IsPointerOverUI(touch)) return;

        Instantiate(DataHandler.Instance.GetFurniture(), pose.position, pose.rotation);
    }

    bool IsPointerOverUI(Touch touch)
    {
        // using UnityEngine.EventSystems;
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.position.x, touch.position.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    void CrosshairCalculation()
    {
        // 할당된 인자 position을 뷰포트좌표에서 화면좌표로 변환
        // 뷰 포트 좌표계는 화면에 글자나 2D 이미지를 표시하기 위한 좌표계로
        // 화면의 왼쪽 아래를 (0,0) 오른쪽 위를 (1, 1)로 하는 평면 상대 좌표계
        // 화면좌표는 단말기의 좌표계
        // 즐겨찾기 참고
        Vector3 origin = arCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));

        // 터치한 위치로 카메라에서 ray를 쏨
        Ray ray = arCam.ScreenPointToRay(origin);

        // ray가 충돌하면 결과를 _hits리스트에 저장하고 true반환하여 몸체 실행
        if (_raycastManager.Raycast(ray, _hits))
        {
            // 충돌지점을 Marker의 위치로 설정하고 x축기준으로 90도 회전
            pose = _hits[0].pose;
            crosshair.transform.position = pose.position;
            crosshair.transform.eulerAngles = new Vector3(90, 0, 0);
        }
    }
}
