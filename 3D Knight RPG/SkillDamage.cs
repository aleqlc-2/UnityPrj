using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬효과 프리펩에 부착된 스크립트
public class SkillDamage : MonoBehaviour
{
    public LayerMask enemyLayer;

    public float radius = 0.5f;
    public float damageCount = 10f;

    private EnemyHealth enemyHealth;

    private bool collided;

    void Update()
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
            enabled = false;
        }
    }
}
