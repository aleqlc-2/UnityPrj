using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUI : MonoBehaviour
{
    private BoxCollider boxCollider;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>(); // 이 스크립트가 부착된 오브젝트의 RectTransform값을 할당

        boxCollider = gameObject.AddComponent<BoxCollider>();

        boxCollider.size = rectTransform.sizeDelta; // 버튼의 크기에 맞춰 콜라이더의 크기 설정
    }
}
