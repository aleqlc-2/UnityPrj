using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackDamageTrigger : MonoBehaviour
{
    public float damage = 1f;

    public bool isPlayer;

    private void OnTriggerEnter(Collider target)
    {
        if (isPlayer)
        {
            if (target.tag == TagManager.ENEMY_TAG || target.tag == TagManager.BOSS_TAG)
            {
                target.GetComponent<HealthScript>().ApplyDamage(damage);
            }
        }
        else
        {
            if (target.tag == TagManager.PLYAER_TAG)
            {
                target.GetComponent<HealthScript>().ApplyDamage(damage);
            }
        }
    }
}
