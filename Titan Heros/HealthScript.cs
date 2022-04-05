using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public float health = 100f;

    private CharacterAnimation characterAnim;

    public bool isEnemy;

    public UIHealth displayHealth;

    void Awake()
    {
        characterAnim = GetComponent<CharacterAnimation>();
    }

    public void ApplyDamage(float damage)
    {
        health -= damage;

        if (displayHealth != null) displayHealth.DisplayHealthValue(health);

        if (health <= 0)
        {
            characterAnim.Dead();
        }
        else
        {
            if (isEnemy)
            {
                characterAnim.Hit();
            }
        }
    }
}
