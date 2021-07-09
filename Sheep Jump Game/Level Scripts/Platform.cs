using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Platform 2 Platform 프리팹에 들어간 스크립트
public class Platform : MonoBehaviour
{
    [SerializeField] private Transform[] spikes;
    [SerializeField] private GameObject coin;

    private bool fallDown;
    
    void Start()
    {
        ActivatePlatform();
    }

    private void ActivatePlatform()
    {
        int chance = UnityEngine.Random.Range(0, 100);
        
        if (chance < 70)
        {
            int type = UnityEngine.Random.Range(0, 8);

            if (type == 0) { ActivateSpike(); }
            else if (type == 1) { fallDown = true; }
            else if (type == 2) { fallDown = true; }
            else if (type == 3) { fallDown = true; }
            else if (type == 4) { fallDown = true; }
            else if (type == 5) { fallDown = true; }
            else if (type == 6) { AddCoin(); }
            else if (type == 7) { AddCoin(); }
        }
    }

    void ActivateSpike()
    {
        int index = UnityEngine.Random.Range(0, spikes.Length);
        spikes[index].gameObject.SetActive(true);

        // SetLoops의 -1은 무한루프를 뜻함
        spikes[index].DOLocalMoveY(0.7f, 1.3f).SetLoops(-1, LoopType.Yoyo).SetDelay(
            UnityEngine.Random.Range(3f, 5f));
    }

    void AddCoin()
    {
        GameObject c = Instantiate(coin);
        c.transform.position = transform.position;
        c.transform.SetParent(transform); // child화 했으니
        c.transform.DOLocalMoveY(1f, 0f); // 로컬무브로
    }

    void OnCollisionEnter(Collision target)
    {
        if (target.gameObject.tag == "Player")
        {
            if (fallDown)
            {
                fallDown = false;
                Invoke("InvokeFalling", 1.5f);
            }
        }
    }

    void InvokeFalling()
    {
        gameObject.AddComponent<Rigidbody>();
    }
} // class
