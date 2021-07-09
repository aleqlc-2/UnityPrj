using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField]
    private Transform startPosition;

    private float line_Width = 0.05f;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = line_Width;
        lineRenderer.enabled = false;
    }

    // draw line
    public void RenderLine(Vector3 endPosition, bool enableRenderer)
    {
        if (enableRenderer)
        {
            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;
            }
            lineRenderer.positionCount = 2;
        }
        else
        {
            lineRenderer.positionCount = 0;
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
            }
        }

        if (lineRenderer.enabled)
        {
            Vector3 temp = startPosition.position;
            temp.z = -10f; // Hook Parent -> Line Renderer -> Position의 Z
                           // 이 코드 안쓰면 로프배경에 가려서 안보임.
                           // 시작점을 Z축기준 -10만큼 당겨서 배경위로 보이게끔 해주는것.

            startPosition.position = temp;

            // 밑에 세줄 없어도 동작 같음..
            temp = endPosition;
            temp.z = 0f;
            endPosition = temp;

            lineRenderer.SetPosition(0, startPosition.position);
            lineRenderer.SetPosition(1, endPosition);
        }
    }
}
