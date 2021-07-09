using UnityEngine;

public class RopeCutter : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // 마우스 클릭한 곳에서 화면 좌상단으로 ray를 쏜다..?
            RaycastHit2D hit = Physics2D.Raycast
                (Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.tag == "Link")
                {
                    Destroy(hit.collider.gameObject);
                    // Destroy(hit.transform.parent.gameObject, 2f); //2초 뒤 Rope와 함께 Destroy
                }
            }
        }    
    }
}
