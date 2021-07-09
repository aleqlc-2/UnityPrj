using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{
    private Rigidbody2D myBody;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 한칸씩 움직일것이므로 speed와 time은 안곱함
        if (Input.GetKeyDown(KeyCode.RightArrow))
            myBody.MovePosition(myBody.position + Vector2.right);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            myBody.MovePosition(myBody.position + Vector2.left);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            myBody.MovePosition(myBody.position + Vector2.up);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            myBody.MovePosition(myBody.position + Vector2.down);
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "Car")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
