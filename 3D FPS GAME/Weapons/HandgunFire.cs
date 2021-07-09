using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandgunFire : MonoBehaviour
{
    public GameObject theGun;
    public GameObject muzzleFlash;
    public AudioSource gunFire;
    public bool isFiring = false;
    public AudioSource emptySound;
    public float targetDistance;
    public int damageAmount = 5;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (GlobalAmmo.handgunAmmo < 1)
            {
                emptySound.Play();
            }
            else
            {
                if (isFiring == false)
                    StartCoroutine(FiringHandgun());
            }
        }
    }

    IEnumerator FiringHandgun()
    {
        isFiring = true;
        GlobalAmmo.handgunAmmo -= 1;

        RaycastHit theShot;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out theShot))
        {
            targetDistance = theShot.distance;
            theShot.transform.SendMessage("DamageEnemy", damageAmount, SendMessageOptions.DontRequireReceiver);
            // DamageEnemy 메소드에 damageAmount 값 전달
        }

        theGun.GetComponent<Animator>().Play("HandgunFire");
        muzzleFlash.SetActive(true);
        gunFire.Play();
        
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
        yield return new WaitForSeconds(0.25f); // 0.3f초 마다 발사가능
        theGun.GetComponent<Animator>().Play("New State"); // 이 코드 없으면 첫 발사만 "HandgunFire" 동작함
        isFiring = false;
    }
}
