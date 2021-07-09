using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBoxControl : MonoBehaviour
{
    GameObject textBox1;
    bool textBox1view = true;

    void Start()
    {
        textBox1 = GameObject.Find("Canvas_TextBox1"); // 오브젝트 실제 이름으로 찾음
    }

    // model 클릭할 때
    void OnMouseDown() // model에 콜라이더 넣어야함
    {
        if (textBox1view == true)
        {
            textBox1view = false;
            textBox1.SetActive(false);
        }
        else
        {
            textBox1view = true;
            textBox1.SetActive(true);
        }
    }  
}
