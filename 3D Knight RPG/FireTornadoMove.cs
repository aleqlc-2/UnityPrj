using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTornadoMove : MonoBehaviour
{
    public LayerMask enemyLayer;

    public float radius = 1f;
    public float damageCount = 20f;
    private float speed = 3f;

    public GameObject fireExplosion;

    private EnemyHealth enemyHealth;

    private bool collided;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // 플레이어가 바라보는 방향으로 스킬효과가 날아가도록
        transform.rotation = Quaternion.LookRotation(player.transform.forward);
    }

    void Update()
    {
        Move();
        CheckForDamage();
    }

    private void Move()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void CheckForDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        foreach (Collider c in hits)
        {
            enemyHealth = c.gameObject.GetComponent<EnemyHealth>();
            collided = true;
        }

        if (collided)
        {
            enemyHealth.TakeDamage(damageCount);
            Vector3 temp = transform.position;
            temp.y += 2f;
            Instantiate(fireExplosion, temp, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
