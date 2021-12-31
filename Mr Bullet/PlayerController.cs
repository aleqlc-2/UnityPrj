using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float rotateSpeed = 100f, bulletSpeed = 17f;

    public int ammo = 4;

    private Transform handPos;
    private Transform firePos1, firePos2;

    private LineRenderer lineRenderer;

    public GameObject bullet;

    private GameObject crosshair;

    [HideInInspector] public bool isAiming = false;

    public AudioClip gunShot;

    void Awake()
    {
        handPos = GameObject.Find("LeftArm").transform;
        firePos1 = GameObject.Find("FirePos1").transform;
        firePos2 = GameObject.Find("FirePos2").transform;

        crosshair = GameObject.Find("Crosshair");
        crosshair.SetActive(false);

        lineRenderer = GameObject.Find("Gun").GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void FixedUpdate()
    {
        if (!IsMouseOverUI()) // UI 아닌곳을 눌렀을때만 Aim하도록
        {
            // Update로 가면 Aim 떨림
            if (Input.GetMouseButton(0))
            {
                isAiming = true;
                Aim();
            }
        }
    }

    void Update()
    {
        if (!IsMouseOverUI()) // UI 아닌곳을 눌렀을때만 Shoot하도록
        {
            //FixedUpdate로 가면 인식안됨
            if (Input.GetMouseButtonUp(0))
            {
                if (ammo > 0)
                    Shoot();
                else
                {
                    lineRenderer.enabled = false;
                    crosshair.SetActive(false);
                }
            }
        }
    }

    private void Aim()
    {
        // 마우스 찍은 위치로 팔 회전
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - handPos.position; // x, y
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        handPos.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);

        // sprite editor에서 앵커를 조정하여 라인렌더러와 위치맞출 수 있음
        crosshair.SetActive(true);
        crosshair.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePos1.position);
        lineRenderer.SetPosition(1, firePos2.position);
    }

    private void Shoot()
    {
        crosshair.SetActive(false);
        lineRenderer.enabled = false;

        GameObject b = Instantiate(bullet, firePos1.position, Quaternion.identity);

        if (transform.localScale.x > 0) // 플레이어가 우측을 보고 있으면
            b.GetComponent<Rigidbody2D>().AddForce(firePos1.right * bulletSpeed, ForceMode2D.Impulse);
        else // 플레이어가 좌측을 보고 있으면
            b.GetComponent<Rigidbody2D>().AddForce(-firePos1.right * bulletSpeed, ForceMode2D.Impulse);

        ammo--;
        FindObjectOfType<GameManager>().CheckBullets();

        SoundManager.instance.PlaySoundFX(gunShot, 0.3f);

        Destroy(b, 2f);
    }

    // UI 위에 마우스 올리면 true반환
    private bool IsMouseOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}
