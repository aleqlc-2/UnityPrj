using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordTimer : MonoBehaviour
{
    public WordManager wordManager;

    public float wordDelay = 1.5f;
    private float nextWordTime = 0f;

    private void Update()
    {
        // deltaTime, fixedDeltaTime은 단어가 하나만 생성됨..
        if (Time.time >= nextWordTime)
        {
            wordManager.AddWord();
            nextWordTime = Time.time + wordDelay;
            wordDelay *= .99f;
        }
    }
}
