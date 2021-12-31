using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Animator anim;

    private AudioSource audioManager;

    public GameObject explosion;

    public float speed = 1f;

    private bool canMove = true;
    private bool canShoot = false;

    public ParticleSystem rightMuzzle, leftMuzzle, rightFire, leftFire;
    private ParticleSystem.EmissionModule right_Muzzle_Emission, left_Muzzle_Emission, right_Fire_Emission, left_Fire_Emission;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        anim.Play("Walk");
        audioManager = GetComponent<AudioSource>();

        // ParticleSystem.EmissionModule에 ParticleSystem의 emission을 넣고
        right_Muzzle_Emission = rightMuzzle.emission;
        left_Muzzle_Emission = leftMuzzle.emission;

        // ParticleSystem.EmissionModule의 rateOverTime을 0으로
        right_Muzzle_Emission.rateOverTime = 0f;
        left_Muzzle_Emission.rateOverTime = 0f;

        // ParticleSystem.EmissionModule에 ParticleSystem의 emission을 넣고
        right_Fire_Emission = rightFire.emission;
        left_Fire_Emission = leftFire.emission;

        // ParticleSystem.EmissionModule의 rateOverTime을 0으로
        right_Fire_Emission.rateOverTime = 0f;
        left_Fire_Emission.rateOverTime = 0f;
    }

    void Update()
    {
        Move();
        CheckToShoot();
    }

    private void Move()
    {
        if (canMove)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (!isGrounded() || CheckFront()) // 허공에 있거나 앞에 물체가 있으면
            {
                anim.Play("Idle");
                canMove = false; // 움직임을 멈추고
                LeanTween.rotateAroundLocal(gameObject, Vector3.up, 180f, 0.5f).setOnComplete(CompleteMove); // rotate
            }
        }
    }

    private bool isGrounded()
    {
        // Vector3.down 방향으로 0.1f 만큼쏴서 레이가 충돌하면 true이므로 땅에 닿은것.
        return Physics.Raycast(transform.position + transform.forward * 0.4f + transform.up * 0.1f, Vector3.down, 0.1f);
    }

    private bool CheckFront()
    {
        // transform.forward 방향으로 0.1f 만큼쏴서 레이가 충돌하면 true이므로 앞에 물체가 있는것.
        return Physics.Raycast(transform.position + transform.forward * 0.4f + transform.up * 0.5f, transform.forward, 0.1f);
    }

    private void CompleteMove() // rotate가 끝나면 다시 Walk
    {
        anim.Play("Walk");
        canMove = true;
    }

    private void CheckToShoot()
    {
        if (canShoot)
        {
            if (!audioManager.isPlaying)
            {
                audioManager.Play(); // 총쏘는소리 on
            }
            right_Muzzle_Emission.rateOverTime = left_Muzzle_Emission.rateOverTime = 10f; // 총알 큐브(Muzzle 파티클 오브젝트)
            right_Fire_Emission.rateOverTime = left_Fire_Emission.rateOverTime = 30f; // 총구 화염(Fire 파티클 오브젝트)
        }
        else // off
        {
            audioManager.Stop();
            right_Muzzle_Emission.rateOverTime = left_Muzzle_Emission.rateOverTime = 0f; // 총알 큐브(Muzzle 파티클 오브젝트)
            right_Fire_Emission.rateOverTime = left_Fire_Emission.rateOverTime = 0f; // 총구 화염(Fire 파티클 오브젝트)
        }
    }

    // 콜라이더를 2개 부착해서 하나는 isTrigger 언체크해서 땅 뚫고 떨어져버리지 않도록 했음.
    private void OnTriggerEnter(Collider target) // 앞으로 길게 뻗은 콜라이더(isTrigger 체크)
    {
        if (target.gameObject.name == "Robot") // 플레이어를 보면
        {
            canShoot = true; // shoot
        }
    }

    private void OnTriggerExit(Collider target) // 앞으로 길게 뻗은 콜라이더(isTrigger 체크)
    {
        if (target.gameObject.name == "Robot") // 플레이어를 벗어나면
        {
            canShoot = false; // off
        }
    }

    private void Damage()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // 파티클의 Collision에 체크해야함, Collides With에서 충돌대상 정할수 있고
    // Enable Dynamic Colliders와 Send Collision Messages에 체크해야함
    // 자기가 쏜 파티클이건 플레이어가 쏜 파티클이건 전부 인식함
    // 이 스크립트가 부착된 오브젝트가 파티클에 맞았을때
    private void OnParticleCollision(GameObject target) // target은 파티클을 맞은 이 스크립트가 부착된 오브젝트
    {
        // Muzzle에는 콜라이더없어도됨. 하지만 이 스크립트가 부착된 오브젝트(부모개체)에는 콜라이더가 있어야함
        if (target.name == "Muzzle")
        {
            Damage();
        }
    }
}
