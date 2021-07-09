using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBowScript : MonoBehaviour
{
    private Rigidbody myBody;

    public float speed = 30f;
    public float deactivate_Timer = 3f;
    public float damage = 50f;

    void Awake()
    {
        myBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Invoke("DeactivateGameObject", deactivate_Timer);
    }

    public void Launch(Camera mainCamera)
    {
        myBody.velocity = mainCamera.transform.forward * speed; // 방향(Vector3) * 스피드
        transform.LookAt(transform.position + myBody.velocity); // 날아가는 방향을 바라보도록
    }

    void DeactivateGameObject()
    {
        if (gameObject.activeInHierarchy) gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider target)
    {
        if (target.tag == Tags.ENEMY_TAG)
        {
            target.GetComponent<HealthScript>().ApplyDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
