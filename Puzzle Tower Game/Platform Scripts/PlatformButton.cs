using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformButton : MonoBehaviour
{
    private RotatingPlatform rotatingPlatform;

    void Awake()
    {
        // GetComponentInParent 메서드로 부모계층의 RotatingPlatform 스크립트를 가져옴
        // button white에 닿으면 부모계층까지 모두 Rotate
        rotatingPlatform = GetComponentInParent<RotatingPlatform>();
    }

    void OnTriggerEnter(Collider target)
    {
        if (target.CompareTag(Tags.PLAYER_TAG))
        {
            rotatingPlatform.ActivateRotation();
        }
    }
}
