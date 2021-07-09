using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// 터치했을때 그 위치에 ray쏴서 Polygon있으면 그 닿은 위치에 Ball 생성
public class SetARBall : MonoBehaviour
{
    public ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;
            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = s_Hits[0].pose;
                // raycastPrefab은 인스펙터창에서 할당
                Instantiate(m_RaycastManager.raycastPrefab, hitPose.position, hitPose.rotation);
            }
        }
    }
}
