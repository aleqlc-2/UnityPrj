using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Transform[] children;

    [SerializeField] private bool deactivateInStart;

    void Start()
    {
        // Entrance Platform의 하위계층의 모든 Transform을 배열로 저장
        // GetComponentInChildren가 아니라 GetComponentsInChildren로 불러와야함
        children = transform.GetComponentsInChildren<Transform>();

        // 시작시 문이 닫혀있다면
        if (deactivateInStart)
        {
            OpenDoors(); // 문을 연다
        }
    }

    // 하나씩 불러와서 Door태그를 가진 개체를 찾아 문을 연다
    public void OpenDoors()
    {
        foreach (Transform c in children)
        {
            if (c.CompareTag(Tags.DOOR_TAG))
            {
                c.gameObject.GetComponent<Collider>().isTrigger = true;
            }
        }
    }
}
