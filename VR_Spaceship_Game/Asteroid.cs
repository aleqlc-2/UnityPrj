using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    void Start()
    {
        Rigidbody r = GetComponent<Rigidbody>();
        r.velocity = Random.onUnitSphere * Random.Range(0.5f, 1f); // Vector3 * 이동스피드
        r.angularVelocity = Random.onUnitSphere * Random.Range(0.5f, 0.9f); // Vector3 * 회전스피드
    }

    void OnParticleCollision() // 파티클(미사일)에 부딪히면 운석 파괴됨
    {
        Destroy(this.gameObject);
    }
}
