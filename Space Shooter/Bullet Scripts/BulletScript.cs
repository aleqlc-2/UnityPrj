using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed = 5f;
    public float deactivate_Timer = 7f;

    [HideInInspector]
    public bool is_EnemyBullet = false;

    void Start()
    {
        if (is_EnemyBullet)
        {
            speed *= -1f;
        }

        Invoke("DeactivateGameObject", deactivate_Timer);
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 temp = transform.position;
        temp.x += Time.deltaTime * speed;
        transform.position = temp;
    }

    void DeactivateGameObject()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "Bullet" || target.tag == "Enemy")
        {
            gameObject.SetActive(false);
        }
    }

} // class
