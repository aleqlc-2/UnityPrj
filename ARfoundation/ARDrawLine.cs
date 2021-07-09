using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARDrawLine : MonoBehaviour
{
    public Transform _pivotPoint;

    public GameObject _lineRenderPrefab;

    private LineRenderer _lineRenderer;

    public List<LineRenderer> _lineList = new List<LineRenderer>(); // 나중에 지우기 위해

    public Transform _linePool;

    public bool _use; // 라인그리기가 시작되었는지 확인
    public bool _startLine; // 라인렌더러가 사용되고 있는지 확인

    void Start()
    {
        
    }

    void Update()
    {
        if(_use)
        {
            if(_startLine)
            {
                DrawLineContinue(); // press했을때 이 메서드는 계속하여 호출되어
            }
        }
    }

    public void DrawLineContinue()
    {
        // 계속하여 그려야 하므로 이전 지점 저장
        // 0번 인덱스는 MakeLineRenderer()에서 SetPosition했음
        _lineRenderer.positionCount = _lineRenderer.positionCount + 1; // positionCount저장

        // positionCount - 1이 저장된게 아님
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _pivotPoint.position);
    }

    public void MakeLineRenderer()
    {
        GameObject tLine = Instantiate(_lineRenderPrefab);
        tLine.transform.SetParent(_linePool);
        tLine.transform.position = Vector3.zero;
        tLine.transform.localScale = new Vector3(1,1,1);

        _lineRenderer = tLine.GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, _pivotPoint.position);

        _startLine = true; // press했을때 MakeLineRenderer메서드는 한번만 호출될 수 있도록
        _lineList.Add(_lineRenderer);
    }

    public void StartDrawLine()
    {
        _use = true;
        if (!_startLine) // press했을때 MakeLineRenderer메서드는 한번만 호출될 수 있도록
        {
            MakeLineRenderer();
        }
    }

    public void StopDrawLine()
    {
        _use = false;
        _startLine = false;
        _lineRenderer = null; // 나중에 다시 그릴때 새로운 라인렌더러를 사용할 수 있도록
    }
}
