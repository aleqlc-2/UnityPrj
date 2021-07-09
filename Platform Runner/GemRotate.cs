using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemRotate : MonoBehaviour
{
    public int rotateSpeed = 2;

    void Update()
    {
        // esc누르면 timeScale을 0이 되도록 PauseGame클래스에서 코딩해놨으므로
        // esc누르면 회전을 멈춤
        transform.Rotate(0, rotateSpeed * Time.timeScale, 0, Space.World);
    }
}
