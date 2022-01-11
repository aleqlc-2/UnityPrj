using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

// Jump Button에 부착된 스크립트
public class JumpButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // 버튼 누르면 호출, 이 스크립트가 UI 컴포넌트에 부착되었을때만 호출됨
    public void OnPointerDown(PointerEventData eventData)
    {
        if (PlayerJumpScript.instance != null) PlayerJumpScript.instance.SetPower(true);
    }

    // 버튼 떼면 호출, 이 스크립트가 UI 컴포넌트에 부착되었을때만 호출됨
    public void OnPointerUp(PointerEventData eventData)
    {
        if (PlayerJumpScript.instance != null) PlayerJumpScript.instance.SetPower(false);
    }
}
