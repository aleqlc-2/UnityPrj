using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CarManager : MonoBehaviour
{
    public GameObject indicator;
    public GameObject myCar;
    private GameObject placedObject;

    public float relocationDistance = 1.0f;

    ARRaycastManager arManager;

    void Start()
    {
        indicator.SetActive(false);
        arManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        DetectGround();

        if (indicator.activeInHierarchy && Input.touchCount > 0)
		{
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
			{
                if (placedObject == null)
				{
                    placedObject = Instantiate(myCar, indicator.transform.position, indicator.transform.rotation);
				}
				else
				{
                    placedObject.transform.SetPositionAndRotation(indicator.transform.position, indicator.transform.rotation);
				}
			}
			else
			{
                if (Vector3.Distance(placedObject.transform.position, indicator.transform.position) > relocationDistance)
				{
                    placedObject.transform.SetPositionAndRotation(indicator.transform.position, indicator.transform.rotation);
				}
			}
		}
    }

    private void DetectGround()
	{
        // 스크린 정중앙으로 레이발사 하기위해
        Vector2 screenSize = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        List<ARRaycastHit> hitInfos = new List<ARRaycastHit>();

        if (arManager.Raycast(screenSize, hitInfos, TrackableType.Planes))
		{
            indicator.SetActive(true);
            indicator.transform.position = hitInfos[0].pose.position;
            indicator.transform.rotation = hitInfos[0].pose.rotation;
            indicator.transform.position += indicator.transform.up * 0.1f;
		}
		else
		{
            indicator.SetActive(false);
		}
	}
}
