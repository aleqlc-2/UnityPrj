using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    public Rigidbody2D hook;
    public GameObject nextBall;

    public float releaseTime = .15f;
    public float maxDragDistance = 2f;

    private bool isPressed = false;

    void Update()
    {
        if (isPressed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(mousePos, hook.position) > maxDragDistance)
                rb.position = hook.position + (mousePos - hook.position).normalized * maxDragDistance;
            else
                rb.position = mousePos;
        }
    }

    void OnMouseDown() // 콜라이더에 클릭한 경우 호출
    {
        isPressed = true;
        rb.isKinematic = true;
    }

    void OnMouseUp()
    {
        isPressed = false;
        rb.isKinematic = false;

        StartCoroutine(Release());
    }

    IEnumerator Release()
    {
        yield return new WaitForSeconds(releaseTime);

        GetComponent<SpringJoint2D>().enabled = false;
        this.enabled = false; // 날아갈때는 클릭적용안되도록 스크립트를 비활성화

        yield return new WaitForSeconds(2f);

        if (nextBall != null)
        {
            nextBall.SetActive(true);
        }
        else
        {
            Enemy.EnemiesAlive = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
