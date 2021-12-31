using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Animation anim;

    private Camera mainCamera;

    private Rigidbody myBody;

    private float forwardVelocity = 100f;
    private float minimumSpeed = -1500f;

    private float maxHorizontalSpeed = 250f;
    private float current_HorizontalSpeed = 0f;

    private float maxVerticalSpeed = 250f;
    private float current_VerticalSpeed = 0f;

    private float currentRotation = 0f;
    private Vector3 currentAngle;

    public float left_BorderLimitX = 130f;
    public float right_BorderLimitX = 370f;
    public float vertical_UpperLimit = 160f;
    public float vertical_LowerLimit = 40f;

    private float bonus_HorizontalSpeed = 0f;
    private float boost_HorizontalSpeed = 0f;

    private float startSpeed = 40f;

    private bool moving = false;

    private Vector3 storedVelocity;

    private bool speed_Boosted = false;
    private int speed_Boost_Value = 100;
    private float speed_Boost_Timeout = 5f;
    private float speed_Boost_Timer = 0f;

    private EnemyPlaneSpawner enemyPlaneSpawner;

    private BalloonSpawner balloonSpawner;

    private PickupSpawner pickupSpawner;

    private HUDController hudController;

    void Awake()
    {
        anim = GetComponent<Animation>();

        myBody = GetComponent<Rigidbody>();
        myBody.isKinematic = true;

        currentAngle = myBody.transform.eulerAngles;

        mainCamera = Camera.main;

        storedVelocity = new Vector3(0f, 0f, startSpeed);
    }

    private void Start()
    {
        enemyPlaneSpawner = GameObject.Find("EnemyPlaneSpawner").GetComponent<EnemyPlaneSpawner>();

        balloonSpawner = GameObject.Find("AirBalloonSpawner").GetComponent<BalloonSpawner>();

        pickupSpawner = GameObject.Find("PickUpSpawner").GetComponent<PickupSpawner>();

        hudController = GameObject.Find("HUD Controller").GetComponent<HUDController>();
    }

    void FixedUpdate()
    {
        if (moving)
        {
            myBody.velocity = new Vector3(current_HorizontalSpeed, current_VerticalSpeed, myBody.velocity.z);
            myBody.transform.eulerAngles = currentAngle;

            SpeedBoost();
        }
    }

    void Update()
    {
        CheckPlayerBorders();
        CheckControlInput();
        KeyboardInput();
    }

    private void MoveRight()
    {
        currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, 0f, Time.deltaTime),
                                   Mathf.LerpAngle(currentAngle.y, 0f, Time.deltaTime),
                                   Mathf.LerpAngle(currentAngle.z, -70f, Time.deltaTime));

        current_HorizontalSpeed = Mathf.Lerp(current_HorizontalSpeed,
                                             maxHorizontalSpeed + bonus_HorizontalSpeed + boost_HorizontalSpeed,
                                             Time.deltaTime);
    }

    private void MoveLeft()
    {
        currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, 0f, Time.deltaTime),
                                   Mathf.LerpAngle(currentAngle.y, 0f, Time.deltaTime),
                                   Mathf.LerpAngle(currentAngle.z, 70f, Time.deltaTime));

        current_HorizontalSpeed = Mathf.Lerp(current_HorizontalSpeed,
                                             -maxHorizontalSpeed + -bonus_HorizontalSpeed + -boost_HorizontalSpeed,
                                             Time.deltaTime);
    }

    private void MoveUp()
    {
        currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, -35f, Time.deltaTime),
                                   currentAngle.y,
                                   currentAngle.z);

        current_VerticalSpeed = Mathf.Lerp(current_VerticalSpeed, maxVerticalSpeed, Time.deltaTime / 2f);
    }

    private void MoveDown()
    {
        currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, 35f, Time.deltaTime),
                                   currentAngle.y,
                                   currentAngle.z);

        current_VerticalSpeed = Mathf.Lerp(current_VerticalSpeed, -maxVerticalSpeed, Time.deltaTime / 2f);
    }

    private void KeyboardInput()
    {
        if (moving)
        {
            if (Input.GetKey(KeyCode.A)) MoveLeft();

            if (Input.GetKey(KeyCode.D)) MoveRight();

            if (Input.GetKey(KeyCode.W)) MoveUp();

            if (Input.GetKey(KeyCode.S)) MoveDown();
        }
    }

    private void CheckControlInput()
    {
        if (moving)
        {
            // 좌우 아무키도 안누르고 있으면 비행기가 자동으로 수평으로 돌아오게끔
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                current_HorizontalSpeed = Mathf.Lerp(current_HorizontalSpeed, 0f, Time.deltaTime / 0.1f);

                currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, currentAngle.x, Time.deltaTime), // 어차피 0f에서 0f라 0f줘도 됨
                                           Mathf.LerpAngle(currentAngle.y, 0f, Time.deltaTime),
                                           Mathf.LerpAngle(currentAngle.z, 0f, Time.deltaTime));
            }

            // 상하 아무키도 안누르고 있으면 비행기가 자동으로 수직으로 돌아오게끔
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                current_VerticalSpeed = Mathf.Lerp(current_VerticalSpeed, 0f, Time.deltaTime / 0.1f);

                currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, 0f, Time.deltaTime * 2f),
                                           Mathf.LerpAngle(currentAngle.y, 0f, Time.deltaTime),
                                           Mathf.LerpAngle(currentAngle.z, currentAngle.z, Time.deltaTime)); // 어차피 0f에서 0f라 0f줘도 됨
            }
        }
    }

    private void CheckPlayerBorders()
    {
        if (moving)
        {
            if (transform.position.y > vertical_UpperLimit)
            {
                transform.position = new Vector3(transform.position.x, vertical_UpperLimit - 1, transform.position.z);

                current_VerticalSpeed = 0f;
                Input.ResetInputAxes(); // 가상 축에 입력된 모든 값을 초기화
            }

            if (transform.position.y < vertical_LowerLimit)
            {
                transform.position = new Vector3(transform.position.x, vertical_LowerLimit + 1, transform.position.z);

                current_VerticalSpeed = 0f;
                Input.ResetInputAxes();
            }

            if (transform.position.x < left_BorderLimitX)
            {
                transform.position = new Vector3(left_BorderLimitX + 1, transform.position.y, transform.position.z);

                current_HorizontalSpeed = 0f;

                currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, 0f, Time.deltaTime),
                                           Mathf.LerpAngle(currentAngle.y, 0f, Time.deltaTime),
                                           Mathf.LerpAngle(currentAngle.z, 0f, Time.deltaTime * 2f));

                Input.ResetInputAxes();
            }

            if (transform.position.x > right_BorderLimitX)
            {
                transform.position = new Vector3(right_BorderLimitX - 1, transform.position.y, transform.position.z);

                current_HorizontalSpeed = 0f;

                currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, 0f, Time.deltaTime),
                                           Mathf.LerpAngle(currentAngle.y, 0f, Time.deltaTime),
                                           Mathf.LerpAngle(currentAngle.z, 0f, Time.deltaTime * 2f));

                Input.ResetInputAxes();
            }
        }
    }

    public void StartTakeOff()
    {
        anim.Play("TakeOff");
    }

    // 이륙 애니메이션 끝나고 게임 시작
    // Animation탭에서 event등록된 메서드
    public void Resume()
    {
        myBody.velocity = storedVelocity; // velocity 주고
        myBody.isKinematic = false; // 물리효과 활성화하고
        moving = true; // 움직인다

        BoxCollider[] boxColls = GetComponents<BoxCollider>(); // s

        foreach (var b in boxColls)
        {
            b.enabled = true; // 비행기의 모든 콜라이더 활성화
        }

        enemyPlaneSpawner.StartSpawningPlanes();

        balloonSpawner.StartSpawningBalloons();

        pickupSpawner.StartSpawningPickups();

        hudController.ActivateHUD(true);
    }

    private void SpeedBoost()
    {
        if (speed_Boosted)
        {
            speed_Boost_Timer += Time.deltaTime;

            if (speed_Boost_Timer < speed_Boost_Timeout)
            {
                myBody.AddRelativeForce(Vector3.forward * speed_Boost_Value);
            }
            else
            {
                speed_Boosted = false;
                speed_Boost_Timer = 0f;

                myBody.velocity = storedVelocity;
                myBody.isKinematic = false;
            }
        }
    }

    public void PlayerCrashed()
    {
        speed_Boosted = false;

        myBody.useGravity = true;
        myBody.mass = 2;
        myBody.transform.Rotate(0.2f, 0.2f, 0.2f);

        GetComponent<PlayerController>().enabled = false;

        StartCoroutine(PlayerDiedRestart());
    }

    IEnumerator PlayerDiedRestart()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fuel") { hudController.FuelCollected(); }

        if (other.tag == "ScoreMultiplier") { hudController.IncreaseScore(); }

        if (other.tag == "SpeedBoost") { speed_Boosted = true; }

        if (other.tag == "enemyAir") { PlayerCrashed(); }
    }
}
