using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJumpScript : MonoBehaviour
{
    public static PlayerJumpScript instance;

    private Rigidbody2D myBody;

    private Animator anim;

    [SerializeField] private float forceX, forceY;
    private float thresholdX = 7f;
    private float thresholdY = 14f;

    private bool setPower, didJump;

    private int score;

    private Slider powerBar;
    private float powerBarThreshold = 10f;
    private float powerBarValue = 0f;

    void Awake()
    {
        MakeInstance();
        Initialize();
    }

    private void MakeInstance()
    {
        if (instance == null) instance = this;
    }

    private void Initialize()
    {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        powerBar = GameObject.Find("Power Bar").GetComponent<Slider>();

        powerBar.minValue = 0f;
        powerBar.maxValue = 10f;
        powerBar.value = powerBarValue;
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (didJump)
        {
            didJump = false;

            if (target.tag == "Platform")
            {
                // 점프 후 다른 플랫폼에 착지했을때 카메라 이동과 새로운 플랫폼 생성
                if (GameManager.instance != null) GameManager.instance.CreateNewPlatformAndLerp(target.transform.position.x);

                anim.SetBool("Jump", false);

                score++;
                GameManager.instance.scoreTxt.text = score.ToString();
            }
        }
    }

    void Update()
    {
        SetPower();
    }

    private void SetPower()
    {
        if (setPower)
        {
            forceX += thresholdX * Time.deltaTime;
            forceY += thresholdY * Time.deltaTime;

            if (forceX > 6.5f) forceX = 6.5f;
            if (forceY > 13.5f) forceY = 13.5f;

            powerBarValue += powerBarThreshold * Time.deltaTime; // +=
            powerBar.value = powerBarValue;
        }
    }

    public void SetPower(bool setPower)
    {
        this.setPower = setPower;

        if (!setPower) Jump();
    }

    private void Jump()
    {
        myBody.velocity = new Vector2(forceX, forceY);
        forceX = forceY = 0f;
        didJump = true;

        anim.SetBool("Jump", true);

        powerBarValue = 0f;
        powerBar.value = powerBarValue;
    }
}
