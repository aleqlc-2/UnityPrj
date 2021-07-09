using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordDisplay : MonoBehaviour
{
    public Text text;
    public float fallSpeed = 1f;

    public void SetWord(string word)
    {
        text.text = word;
    }

    public void RemoveLetter()
    {
        text.text = text.text.Remove(0, 1);
        text.color = Color.red;
    }

    public void RemoveWord()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        // Time.deltaTime 안곱하면 단어가 엄청 빨리 내려가버림
        transform.Translate(0f, -fallSpeed * Time.deltaTime, 0f);
    }
}
