using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float rotSpeed = 0.1f;

    void Update()
    {
        if (Input.touchCount > 0)
		{
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
			{
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hitInfo;

                // 8번 레이어에 등록된것만 검출
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << 8))
				{
                    Vector3 deltaPos = touch.deltaPosition;

                    // 터치로 드래그하면 y축을 기준으로 회전하도록 
                    transform.Rotate(transform.up, deltaPos.x * -1.0f * rotSpeed);
				}
			}
		}
    }
}
