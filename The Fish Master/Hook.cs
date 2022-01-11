using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hook : MonoBehaviour
{
    public Transform hookedTransform;

    private Camera mainCamera;

    private Collider2D coll;

    private bool canMove;

    private int length;
    private int strength;
    private int fishCount;

    private Tweener cameraTween;

    private List<Fish> hookedFishes;

    void Awake()
    {
        mainCamera = Camera.main;
        coll = GetComponent<Collider2D>();
        hookedFishes = new List<Fish>();
    }

    void Update()
    {
        if (canMove && Input.GetMouseButton(0))
        {
            Vector3 vector = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position;
            position.x = vector.x;
            transform.position = position;
        }
    }

    // 버튼 전체화면 덮어서 투명하게 만들어서 화면 아무데나 클릭하면 이 메서드 호출
    public void StartFishing()
    {
        length = IdleManager.instance.length - 20;
        strength = IdleManager.instance.strength;
        fishCount = 0;

        float time = (-length) * 0.1f; // 5

        // OnUpdate는 DOMoveY가 끝나서 OnComplete가 호출되기 전까지 계속해서 호출됨
        cameraTween = mainCamera.transform.DOMoveY(length, 1 * time * 0.25f, false).OnUpdate(delegate
        {
            Debug.Log("OnUpdate");
            if (mainCamera.transform.position.y <= -11)
                transform.SetParent(mainCamera.transform); // hook을 카메라의 자식개체로 만들어 카메라 따라가도록
        }).OnComplete(delegate
        {
            Debug.Log("OnComplete");
            coll.enabled = true; // 다 내려갔으면 물고기 잡아올려야하므로 콜라이더 활성화
            cameraTween = mainCamera.transform.DOMoveY(0, time * 5, false).OnUpdate(delegate
            {
                Debug.Log("OnUpdateInOnComplete");
                if (mainCamera.transform.position.y >= -25f)
                    StopFishing();
            });
        });

        Debug.Log("out");
        ScreensManager.instance.ChangeScreen(Screens.GAME);
        coll.enabled = false; // 버튼누르면 tween시작되고 바로 이코드 실행되므로 비활성화된채로 시작된거나 다름없게됨
        canMove = true;
        hookedFishes.Clear(); // 낚시가 시작되면 List 클리어
    }

    private void StopFishing()
    {
        Debug.Log("StopFishing");
        canMove = false; // 낚시대 움직일 수 없음
        cameraTween.Kill(false); // 이 코드 없으면 StopFishing() 메서드 계속호출됨, true주면 -25 넘어가는 지점에서 바로 끝나버림
        cameraTween = mainCamera.transform.DOMoveY(0, 2, false).OnUpdate(delegate
        {
            if (mainCamera.transform.position.y >= -11)
            {
                transform.SetParent(null);
                transform.position = new Vector2(transform.position.x, -6);
            }
        }).OnComplete(delegate
        {
            transform.position = Vector2.down * 6;
            coll.enabled = true;
            
            int num = 0;
            for (int i = 0; i < hookedFishes.Count; i++)
            {
                hookedFishes[i].transform.SetParent(null); // 제자리로 되돌려보냄
                hookedFishes[i].ResetFish(); // 제자리로 되돌려보냄
                num += hookedFishes[i].Type.price; // 잡은거 계산
            }

            IdleManager.instance.totalGain = num;
            ScreensManager.instance.ChangeScreen(Screens.END);
        });
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        // strength가 3이므로 한번에 최대 3마리까지만 잡을 수 있음
        if (target.CompareTag("Fish") && fishCount != strength)
        {
            fishCount++;
            Fish component = target.GetComponent<Fish>();
            component.Hooked(); // 잡힌 fish의 콜라이더 비활성화해서 OnTriggerEnter2D 다시 들어오지않도록
            hookedFishes.Add(component);
            target.transform.SetParent(transform); // 잡힌 물고기는 hook을 따라가도록
            target.transform.position = hookedTransform.position; // 잡힌 물고기는 hook을 따라가도록
            target.transform.rotation = hookedTransform.rotation; // hook이 90도 회전한 상태이므로 잡힌 fish도 90도 회전하게됨
            target.transform.localScale = Vector3.one; // 스케일 x값이 -1일때 90도 회전하면 주둥이가 아래를 향해버리므로

            // 90도 회전한 상태에서 shake 5초동안하다가
            target.transform.DOShakeRotation(5, Vector3.forward * 45, 10, 90, false).SetLoops(1, LoopType.Yoyo).OnComplete(delegate
            {
                target.transform.rotation = Quaternion.identity; // 원래 회전값으로
            });

            if (fishCount == strength) StopFishing(); // 3마리 잡으면 낚시종료
        }
    }
}
