using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public LayerMask enemyLayer;
    public float damage = 1f;
    public float radius = 1f;

    private EnemyHealth enemyHealth;
    private bool collided;

    void Update()
    {
        CheckForDamage();
    }

    private void CheckForDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        foreach (Collider h in hits)
        {
            enemyHealth = h.GetComponent<EnemyHealth>();

            if (enemyHealth) collided = true;
        }

        if (collided)
        {
            collided = false;
            enemyHealth.TakeDamage(damage);
            gameObject.SetActive(false);
        }

    }
}
