using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 1f;

    void FixedUpdate()
    {
        Vector2 temp = transform.position;

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            temp.x += moveSpeed * Time.deltaTime;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            temp.x -= moveSpeed * Time.deltaTime;
        }

        if (Input.GetAxisRaw("Vertical") > 0)
        {
            temp.y += moveSpeed * Time.deltaTime;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            temp.y -= moveSpeed * Time.deltaTime;
        }

        transform.position = temp;
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "Enemy")
        {
            GameManager.instance.PlayerDied();
        }

        if (target.tag == "Goal")
        {
            GameManager.instance.PlayerReachedGoal();
        }
    }

} // class
