using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public int enemyHealth = 20;
    public bool enemyDead = false;
    public GameObject enemyAI;
    public GameObject theEnemy; // 박스콜라이더가 있는 개체 사용해야함. 자식에만 콜라이더있어도 안됨.

    void DamageEnemy(int damageAmount)
    {
        enemyHealth -= damageAmount;
    }

    void Update()
    {
        if (enemyHealth <= 0 && enemyDead == false)
        {
            enemyDead = true;
            theEnemy.GetComponent<Animator>().Play("Death");
            enemyAI.SetActive(false);
            theEnemy.GetComponent<LookPlayer>().enabled = false; // 시체가 움직이면서 바라보지 않도록
            GlobalScore.scoreValue += 100;
            GlobalComplete.enemyCount += 1;
        }
    }
}
