using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    private Transform player;

    public GameObject explosion;

    void Awake()
    {
        player = GameObject.Find("Robot").transform;
    }

    void Update()
    {
        if (player)
        {
            // 캐논의 총구가 플레이어를 위치를 따라가며 조준함
            transform.rotation = Quaternion.Slerp(
                                                transform.rotation,
                                                Quaternion.LookRotation(player.position - transform.position),
                                                2f * Time.deltaTime);
        }
    }

    private void Damage()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // 캐논이 파티클에 맞으면
    // Muzzle의 콜라이더는 없어도됨
    // 하지만 이 스크립트가 부착된 오브젝트(부모개체)에는 콜라이더가 있어야함
    private void OnParticleCollision(GameObject target)
    {
        if (target.name == "Muzzle")
        {
            Damage();
        }
    }
}
