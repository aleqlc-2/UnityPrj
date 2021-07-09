using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierAI : MonoBehaviour
{
    public string hitTag;
    public bool lookingAtPlayer = false;
    public GameObject theSoldier;
    public AudioSource fireSound;
    public bool isFiring = false;
    public float fireRate = 1f;
    public int genHurt;
    public AudioSource[] hurtSound;
    public GameObject hurtFlash;

    void Update()
    {
        RaycastHit enemyHit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out enemyHit))
        {
            hitTag = enemyHit.transform.tag;
        }

        if (hitTag == "Player" && isFiring == false)
        {
            StartCoroutine(EnemyFire());
        }
        
        if (hitTag != "Player")
        {
            theSoldier.GetComponent<Animator>().Play("Idle");
            lookingAtPlayer = false;
        }
    }

    IEnumerator EnemyFire()
    {
        isFiring = true;
        theSoldier.GetComponent<Animator>().Play("FirePistol", -1, 0); // 계속 발사
        //theSoldier.GetComponent<Animator>().Play("FirePistol"); // 이 코드쓰면 한번만 발사하고 멈춤
        fireSound.Play();
        lookingAtPlayer = true;
        GlobalHealth.healthValue -= 5;
        hurtFlash.SetActive(true); // 이 코드에서 적이 죽으면 RemoveFlash로 hurtFlash 비활성화

        yield return new WaitForSeconds(0.2f);
        hurtFlash.SetActive(false);
        genHurt = Random.Range(0, 3);
        hurtSound[genHurt].Play();

        yield return new WaitForSeconds(fireRate); // 발사 쿨타임
        isFiring = false;
    }
}
