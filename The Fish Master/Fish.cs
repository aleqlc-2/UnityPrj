using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Fish : MonoBehaviour
{
    [Serializable]
    public class FishType
    {
        public int price;

        public float fishCount;
        public float minLength;
        public float maxLength;
        public float colliderRadius;

        public Sprite sprite;
    }

    private FishType type;
    public FishType Type
    {
        get { return type; }
        set
        {
            type = value;
            coll.radius = type.colliderRadius;
            rend.sprite = type.sprite;
        }
    }

    private CircleCollider2D coll;

    private SpriteRenderer rend;

    private float screenLeft;

    private Tweener tweener;

    void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        rend = GetComponentInChildren<SpriteRenderer>();

        screenLeft = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        Debug.Log(screenLeft); // -4정도인데 스폰은 왜 -3정도에서 되지?
    }

    public void ResetFish()
    {
        if (tweener != null) tweener.Kill(false);

        coll.enabled = true;

        float num = UnityEngine.Random.Range(type.minLength, type.maxLength); // 스폰할 수심 랜덤정하기
        Vector3 position = transform.position; // 현재위치를
        position.x = screenLeft; // 스폰할 x좌표 할당하고
        position.y = num; // 수심 할당하여
        transform.position = position; // 변경

        float num2 = 1;
        float y = UnityEngine.Random.Range(num - num2, num + num2); // 수심을 -1,+1 더 넓혀서 랜덤정하기
        Vector2 v = new Vector2(-position.x, y); // 대각선이동하도록 반대편 위치 정하기

        float num3 = 3; // 반대편까지 도착하는데 3초걸림
        float delay = UnityEngine.Random.Range(0, 2 * num3); // 이동 시작 0 ~ 5 랜덤, SetDelay는 tween이 시작되기 전 딜레이설정

        // SetLoops -1은 무한반복, 1은 한번만 실행
        tweener = transform.DOMove(v, num3, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(delay).OnStepComplete(delegate
        {
            Vector3 localScale = transform.localScale;
            localScale.x = -localScale.x;
            transform.localScale = localScale;
        });
    }

    public void Hooked()
    {
        coll.enabled = false;
        tweener.Kill(false);
    }
}
