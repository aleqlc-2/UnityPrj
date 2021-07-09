using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Space(3)]
    [Header("============== Player Properties ==============")]
    [Space(3)]
    public Transform playerBody;
    public Rigidbody2D rb;
    public SpringJoint2D springJoint;

    [Space(3)]
    [Header("============== Rope Properties ==============")]
    [Space(3)]
    public LineRenderer line;
    public float minDistance; // 플레이어와 로프사이의 최소거리
    [Range(0.1f, 5f)] public float grabSpeed = 1;

    [Space(3)]
    [Header("============== Hook Properties ==============")]
    [Space(3)]
    public Transform currentHook; // 현재 hook의 위치
    private RaycastHit2D hookHit; // return hook border point for grappling
    private bool isGrappling;

    //private Transform hookPoint;

    private bool isDead;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        isGrappling = false;
        line.positionCount = 2; // 시작점, 끝점 2개를 사용가능

        // hookPoint = new GameObject().transform; // 새로운 게임오브젝트를 생성하여 위치를 저장
        // hookPoint.name = "hookPoint";
    }

    void Update()
    {
        if (isDead)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            GrapplingStart();
        }
        else if (Input.GetMouseButton(0))
        {
            Grappling();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            GrapplingEnd();
        }

        SetPlayerBody(); // Set PlayerBody Animation from speed
    }

    private void SetPlayerBody()
    {
        // set PlayerBody local scale according to rigidbody2d speed
        playerBody.localScale = new Vector3(
            Mathf.Clamp(Remap(rb.velocity.magnitude, 0, 20, 1, 1.5f), 1, 1.5f),
            Mathf.Clamp(Remap(rb.velocity.magnitude, 0, 20, 1, 0.5f), 0.5f, 1),
            1
            );

        // velocity는 Vector3이므로 normalized하면 방향이 됨.
        Vector3 dir = rb.velocity.normalized; // get direction of velocity
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // get angle from direction
        playerBody.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // set rotation of PlayerBody
    }

    // ex) lets take percentage of 250 out of 500 how many percentage it will be
    //     so remap function will be
    //     Remap(250, 0, 500, 0, 100); it will return float percentage 50
    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return ((value - from1) / (to1 - from1) * (to2 - from2)) + from2;
    }

    private void GrapplingStart()
    {
        isGrappling = true;

        Vector3 clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);

        // 원과 오버랩되는 hook 저장
        Collider2D[] colliders;
        colliders = Physics2D.OverlapCircleAll(clickedPos, 25, LayerMask.GetMask("Hook"));

        // 가장 가까운 hook 저장
        currentHook = GetNearestHook(colliders, clickedPos);
        
        if (currentHook == null)
        {
            isGrappling = false;
            return;
        }

        // If current is available then raycast to hook position and grab the corner of hook
        hookHit = Physics2D.Raycast(
                        transform.position, // 내 위치에서
                        (currentHook.position - transform.position).normalized, // hook쪽 방향으로
                        100,
                        LayerMask.GetMask("Hook"));

        //hookPoint.parent = hookHit.transform;
        //hookPoint.position = hookHit.point;
        //hookHit.transform.GetComponent<HookController>().enabled = true;

        // Set Spring Joint to hookhit point
        springJoint.connectedAnchor = hookHit.point;

        // turn on the spring joint
        springJoint.enabled = true;

        // If Current hook is Available then Enable true line renderer
        line.enabled = true;
    }

    private void Grappling()
    {
        line.SetPosition(0, transform.position); // 플레이어 위치
        line.SetPosition(1, hookHit.point); // hook hit point

        // reduce distance between hook and player
        // springJoint.distance를 줄이면 달려있는 플레이어의 위치도 변하는듯
        springJoint.distance = Mathf.Lerp(
            springJoint.distance, minDistance, Time.deltaTime * grabSpeed);

        // 플레이어가 hook주변에서 회전하는 것을 막기 위해 리지드바디 속도를 감소시킴
        rb.velocity *= 0.995f;
    }

    private void GrapplingEnd()
    {
        isGrappling = false;
        springJoint.enabled = false;
        line.enabled = false;

        // line point 위치를 리셋
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        //if (hookHit)
        //{
        //    hookHit.transform.GetComponent<HookController>().enabled = false;
        //}
    }

    // 클릭된 위치에서 가장 가까운 hook의 콜라이더 위치를 반환
    private Transform GetNearestHook(Collider2D[] colliders, Vector3 clickedPos)
    {
        int index = 0;

        float dist = Mathf.Infinity;
        float minDist = Mathf.Infinity;

        // 클릭된 위치와 가장 가까운 거리에 있는 Hook을 찾는다
        for (int i = 0; i < colliders.Length; i++)
        {
            dist = Vector3.Distance(colliders[i].transform.position, clickedPos);
            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        if (colliders.Length > 0)
        {
            return colliders[index].transform; // hook의 위치를 반환
        }

        return null;
    }

    public void HitHook(Transform hook)
    {
        if (hook.tag == "Hook")
        {
            // Normal Hook hit
            GameSceneScript.instance.InitPopUpText(hook.position, 100);
        }

        if (hook.tag == "EnemyHook")
        {
            // Life Decreased. replay game
            SceneManager.LoadScene(1);
        }
    }
}
