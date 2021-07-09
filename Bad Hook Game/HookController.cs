//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class HookController : MonoBehaviour
//{
//    public bool isActive;
//    private Transform target;
//    private Transform parent;
//    private Vector3 prev;

//    private void Awake()
//    {
//        this.enabled = false;
//    }

//    private void OnEnable()
//    {
//        isActive = true;

//        if (parent != null)
//        {
//            prev = transform.parent.eulerAngles;
//            RotateTowards(PlayerController.instance.transform.position);
//            transform.eulerAngles = prev;
//        }
//    }

//    void Start()
//    {
//        target = PlayerController.instance.transform;
//        parent = transform.parent;
//        prev = parent.eulerAngles;
//        RotateTowards(target.position);
//        transform.eulerAngles = prev;
//    }

//    private void RotateTowards(Vector2 target)
//    {
//        float offset = 90f; // rotating to Z position
//        Vector2 direction = target - (Vector2)transform.position; // target으로 향하는 방향
//        direction.Normalize();

//        // hook의 현재 angle
//        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

//        // set parent rotation
//        parent.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
//    }

//    void Update()
//    {
//        RotateTowards(target.position);
//    }

//    private void OnDisable()
//    {
//        isActive = false; // hook을 비활성화

//        // 플레이어가 hook을 벗어날 때
//        parent.rotation = Quaternion.Euler(transform.eulerAngles);
//        transform.rotation = Quaternion.Euler(parent.eulerAngles);
//    }
//}
