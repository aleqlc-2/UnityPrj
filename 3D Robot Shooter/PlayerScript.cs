using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Direction
{
    LEFT,
    RIGHT
}

public class PlayerScript : MonoBehaviour
{
    private Animator anim;

    private AudioSource audioManager;

    private Direction dir = Direction.RIGHT;

    public GameObject missile;

    public Transform leftArm, rightArm;
    public Transform missilePoint;

    public Light leftLight, rightLight;

    public float speed = 4f;
    private float jumpTimer = 0f;

    public ParticleSystem rightMuzzle, leftMuzzle, rightFire, leftFire, boost;
    private ParticleSystem.EmissionModule right_Muzzle_Emission, left_Muzzle_Emission, right_Fire_Emission, left_Fire_Emission;
    private ParticleSystem.MainModule boostMain;

    private Rigidbody myBody;

    private ConstantForce constForce;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>(); // Valken 오브젝트의 Animator 컴포넌트
        anim.Play("Walk");

        audioManager = GetComponent<AudioSource>();

        myBody = GetComponent<Rigidbody>();

        constForce = myBody.GetComponent<ConstantForce>(); // 왜 리지드바디로 가져오는거지?

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

        boostMain = boost.main; // ParticleSystem.MainModule에 ParticleSystem.main을 넣음
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (!LeanTween.isTweening(gameObject)) // 이 코드 없으면 360도 회전함
            {
                if (isGrounded())
                    anim.Play("Walk");
                else
                    anim.Play("Idle");
                
                if (dir != Direction.LEFT) // 오른쪽을 보고있다면
                {
                    LeanTween.rotateAroundLocal(gameObject, Vector3.up, 180f, 0.3f).setOnComplete(TurnLeft); // 왼쪽으로 회전
                }
                else // 왼쪽을 보고있다면
                {
                    transform.Translate(Vector3.forward * speed * Time.deltaTime); // 왼쪽으로 이동
                }
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (!LeanTween.isTweening(gameObject)) // 이 코드 없으면 360도 회전함
            {
                if (isGrounded())
                    anim.Play("Walk");
                else
                    anim.Play("Idle");

                if (dir != Direction.RIGHT) // 왼쪽을 보고있다면
                {
                    LeanTween.rotateAroundLocal(gameObject, Vector3.up, -180f, 0.3f).setOnComplete(TurnRight); // 오른쪽으로 회전
                }
                else // 오른쪽을 보고있다면
                {
                    transform.Translate(Vector3.forward * speed * Time.deltaTime); // 오른쪽으로 이동
                }
            }
        }
        else
        {
            anim.Play("Idle");
        }

        // Arm 회전
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            rightArm.Rotate(Vector3.back * 200f * Time.deltaTime);
            leftArm.Rotate(Vector3.back * 200f * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            rightArm.Rotate(Vector3.forward * 200f * Time.deltaTime);
            leftArm.Rotate(Vector3.forward * 200f * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.Z))
        {
            constForce.force = Vector3.zero; // 이 코드 없으면 엄청 느리게 날아감

            if (myBody.velocity.y < 4f) // 날아가는 속도에 가속도가 붙지 않고 일정한 속도로 날아가도록
                myBody.AddRelativeForce(Vector3.up * 20f);

            if (!boostMain.loop)
            {
                boost.Play(); // boost 파티클 효과
                boostMain.loop = true; // 날아가는동안에는 계속하여 boost 파티클 실행되도록
            }
        }
        else
        {
            constForce.force = new Vector3(0f, -10f, 0f); // 초기화
            boostMain.loop = false; // boost 파티클 효과 종료
        }

        if (Input.GetKey(KeyCode.X))
        {
            if (!audioManager.isPlaying)
            {
                audioManager.Play(); // 총쏘는소리 on
                StartCoroutine("LightControl");
            }
            right_Muzzle_Emission.rateOverTime = left_Muzzle_Emission.rateOverTime = 10f; // 총알 큐브(Muzzle 파티클 오브젝트)
            right_Fire_Emission.rateOverTime = left_Fire_Emission.rateOverTime = 30f; // 총구 화염(Fire 파티클 오브젝트)
        }
        else
        {
            audioManager.Stop(); // 총쏘는소리 off
            right_Muzzle_Emission.rateOverTime = left_Muzzle_Emission.rateOverTime = 0f;
            right_Fire_Emission.rateOverTime = left_Fire_Emission.rateOverTime = 0f;
            leftLight.intensity = rightLight.intensity = 0f;
            StopCoroutine("LightControl");
        }

        if (Input.GetKeyDown(KeyCode.C)) LaunchMissile();
    }

    private IEnumerator LightControl()
    {
        while (true)
        {
            leftLight.intensity = rightLight.intensity = 1f; // 발사할때불빛 on
            yield return new WaitForSeconds(0.3f);
            leftLight.intensity = rightLight.intensity = 0f; // 발사할때불빛 off
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void LaunchMissile()
    {
        if (!LeanTween.isTweening(gameObject))
        {
            Vector3 pos = transform.position;

            if (dir == Direction.RIGHT)
            {
                pos.x += 1f;
                pos.y += 1f;
            }

            if (dir == Direction.LEFT)
            {
                pos.x -= 1f;
                pos.y += 1f;
            }

            for (int i = 0; i < 5; i++)
            {
                Vector3 origin = pos + Vector3.up * Random.Range(-1f, 1f) + Vector3.left * Random.Range(-1f, 1f);

                GameObject temp = Instantiate(missile, origin, Quaternion.AngleAxis(dir == Direction.RIGHT ? 0f : 180f, Vector3.up));

                Vector3 tarPos = missilePoint.position + missilePoint.forward * 20f + missilePoint.up * Random.Range(-1f, 1f);

                // temp오브젝트에 달린 스크립트의 LaunchMissile 메서드로 tarPos 전달하며 호출. 이 스크립트의 LaunchMissile 메서드로는 send안함
                temp.SendMessage("LaunchMissile", tarPos);
            }
        }
    }

    private bool isGrounded()
    {
        // Vector3.down 방향으로 0.1f 만큼쏴서 레이가 충돌하면 true이므로 땅에 닿은것.
        return Physics.Raycast(transform.position + transform.forward * 0.4f + transform.up * 0.1f, Vector3.down, 0.1f);
    }

    private void TurnLeft()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f); // 회전 후 z값을 0으로 리셋하기위함
        dir = Direction.LEFT;
    }

    private void TurnRight()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f); // 회전 후 z값을 0으로 리셋하기위함
        dir = Direction.RIGHT;
    }
}
