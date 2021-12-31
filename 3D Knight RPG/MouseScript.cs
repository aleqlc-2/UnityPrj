using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{
    public Texture2D cursorTexture;

    private CursorMode mode = CursorMode.ForceSoftware;

    private Vector2 hotSpot = Vector2.zero;

    public GameObject mousePoint;
    private GameObject instantiatedMouse;

    void Update()
    {
        //Cursor.SetCursor(cursorTexture, hotSpot, mode);

        if (Input.GetMouseButtonUp(0)) // up
        {
            // 마우스 뗐을때도 Input.mousePosition 적용되는듯
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider is TerrainCollider)
                {
                    Vector3 temp = hit.point;
                    temp.y = 0.25f;

                    if (instantiatedMouse == null)
                    {
                        instantiatedMouse = Instantiate(mousePoint) as GameObject;
                        instantiatedMouse.transform.position = temp;
                    }
                    else // 이미 찍은 포인터가 있으면 그걸 파괴한담에 생성
                    {
                        Destroy(instantiatedMouse);
                        instantiatedMouse = Instantiate(mousePoint) as GameObject;
                        instantiatedMouse.transform.position = temp;
                    }
                }
            }
        }
    }
}
