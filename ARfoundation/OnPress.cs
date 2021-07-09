using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool _pressed = false;

    public ARDrawLine _drawLineST;

    // IPointerDownHandler 인터페이스
    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
    }

    // IPointerUpHandler 인터페이스
    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        _drawLineST.StopDrawLine();
    }

    void Update()
    {
        if (_pressed)
        {
            _drawLineST.StartDrawLine();
        }
    }
}
