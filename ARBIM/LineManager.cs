using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using TMPro;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// AR Ruler
public class LineManager : MonoBehaviour
{
    public GameObject point;

    public LineRenderer lineRenderer;
    //public ARPlacementInteractable placementInteractable;
    
    private LineRenderer line;

    private Vector2 curPos = Vector2.zero;
    private Vector2 prevPos = Vector2.zero;

    public ARRaycastManager raycastMgr;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public ARPlaneManager aRPlaneManager;

    public TextMeshPro mText; // 3D txt는 TextMeshPro
    public TextMeshProUGUI text; // 2D txt는 TextMeshProUGUI

    void Start()
    {
        // placementInteractable.objectPlaced.AddListener(DrawLine);
    }

	void Update()
	{
        if (Input.touchCount > 0)
        {
            var touchZero = Input.GetTouch(0);

            if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled) return;

            if (touchZero.phase == TouchPhase.Began)
            {
                if (raycastMgr.Raycast(touchZero.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    prevPos = hits[0].pose.position;
                    Instantiate(point, hits[0].pose.position, point.transform.rotation);

                    //foreach (var plane in aRPlaneManager.trackables)
                    //{
                    //    plane.gameObject.SetActive(false);
                    //}

                    //aRPlaneManager.enabled = false;
                }
            }
            else if (touchZero.phase == TouchPhase.Moved)
            {
                if (raycastMgr.Raycast(touchZero.position, hits, TrackableType.PlaneWithinPolygon))
				{
                    curPos = hits[0].pose.position;
                }
                
                DrawLine(prevPos, curPos);
            }
        }
    }

	private void DrawLine(Vector2 prevPos, Vector2 curPos)
	{
        line = Instantiate(lineRenderer);
        line.positionCount = 2;

        line.SetPosition(0, prevPos);
        line.SetPosition(1, curPos);

		// 선 위에 text생성하여 거리 표시하기
		Vector3 pointA = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
		Vector3 pointB = lineRenderer.GetPosition(lineRenderer.positionCount - 2);
		float dist = Vector3.Distance(pointA, pointB);
        text.text = dist.ToString();

        TextMeshPro distText = Instantiate(mText);
		distText.text = "" + dist;

		Vector3 directionVector = (pointB - pointA);
		Vector3 normal = point.transform.up;
		Vector3 upd = Vector3.Cross(directionVector, normal).normalized;
		Quaternion rotation = Quaternion.LookRotation(-normal, upd);

		distText.transform.rotation = rotation;
		distText.transform.position = (pointA + directionVector * 0.5f) + upd * 0.05f;
	}
}
