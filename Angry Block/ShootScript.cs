using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class ShootScript : MonoBehaviour
{
    private GameController gc;

    public float power = 2f;

    public int dots = 15;

    private Vector2 startPos;

    private bool shoot, aiming;

    private GameObject Dots;
    public GameObject ballPrefab;
    public GameObject ballsContainer;

    private List<GameObject> projectilesPath; // 포물체 경로들을 저장하는 리스트

    private Rigidbody2D ballBody;

    void Awake()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        Dots = GameObject.Find("Dots");
    }

    void Start()
    {
        Dots = GameObject.Find("Dots");

        // 부모개체인 Dots는 안들어가네
        projectilesPath = Dots.transform.Cast<Transform>().ToList().ConvertAll(t => t.gameObject);

        HideDots();
    }

    void Update()
    {
        ballBody = ballPrefab.GetComponent<Rigidbody2D>();

        // 한 단계내에서 3번까지만 쏠 수 있음
        if (gc.shotCount <= 3 && !IsMouseOverUI())
        {
            Aim();
            Rotate();
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void Aim()
    {
        if (shoot) return;

        // Input.GetAxis는 -1.0f 부터 1.0f 까지의 범위의 값을 반환한다. 조이스틱처럼 부드러운 이동이 필요한 경우
        // Input.GetAxisRaw는 -1, 0, 1 세 가지 값 중 하나가 반환된다. 키보드처럼 즉시 반응해야 하는 경우
        // Update에 넣어서 찍어보면 둘다 클릭하면 1 클릭안하면 0인데?
        if (Input.GetAxis("Fire1") == 1)
        {
            if (!aiming)
            {
                aiming = true;
                startPos = Input.mousePosition; // 마우스 처음 찍은 위치 저장
                gc.CheckShotCount();
            }
            else
            {
                PathCalculation();
            }
        }
        else if (aiming && !shoot) // aiming하다가 마우스 뗐을 때
        {
            aiming = false;
            HideDots();
            StartCoroutine(Shoot());

            // 첫 번째 발사시에만 카메라 회전, 바로위에 Shoot이 코루틴이므로 gc.shotCount++ 되기전에 검사가능
            if (gc.shotCount == 1)
                Camera.main.GetComponent<CameraTransition>().RotateCameraToSide();
        }
    }

    private void PathCalculation()
    {
        // mass가 클수록 약하게 발사되도록 하기위해 ballBody.mass로 나눠줌
        // vel값은 마우스를 처음 클릭한곳으로부터 멀리 이동할수록 커짐
        Vector2 vel = ShootForce(Input.mousePosition) * Time.fixedDeltaTime / ballBody.mass;
        //Debug.Log("ShootForce" + vel);

        for (int i = 0; i < projectilesPath.Count; i++)
        {
            projectilesPath[i].GetComponent<Renderer>().enabled = true;
            float t = i / 15f; // 포물체가 총 16개이므로 15로 나눔
            Vector3 point = DotPath(transform.position, vel, t);
            //Debug.Log("DotPath : " + point);
            point.z = 1;
            projectilesPath[i].transform.position = point;
        }
    }

    // 마우스 처음위치 - 마우스 현재위치 값 리턴
    private Vector2 ShootForce(Vector3 force)
    {
        return (new Vector2(startPos.x, startPos.y) - new Vector2(force.x, force.y)) * power;
    }

    // 포물체 각각의 위치 설정
    private Vector2 DotPath(Vector2 startP, Vector2 startVel, float t)
    {
        // Physics2D.gravity는 (0.0, -9.8)
        // (startVel * t)는 t가 1일때 제일 마지막 포물체의 위치이므로 t를 점점 키워가며 startP에 더해서 다음 포물체를 이전 포물체보다 더 앞으로 보내기 위함
        // (Physics2D.gravity * t * t * 0.5f)는 각각의 포물체에 중력을 적용해 포물선으로 그리기 위함. 이 값 안더하면 직선으로 그려짐
        //Debug.Log((startP));
        //Debug.Log((startVel * t));
        //Debug.Log(Physics2D.gravity * t * t * 0.5f);
        //Debug.Log((startP) + (startVel * t) + (Physics2D.gravity * t * t * 0.5f));
        return (startP) + (startVel * t) + (Physics2D.gravity * t * t * 0.5f);
    }

    private void ShowDots()
    {
        for (int i = 0; i < projectilesPath.Count; i++)
        {
            projectilesPath[i].GetComponent<Renderer>().enabled = true;
            //Debug.Log(projectilesPath[i].gameObject.name);
        }
    }

    private void HideDots()
    {
        for (int i = 0; i < projectilesPath.Count; i++)
        {
            projectilesPath[i].GetComponent<Renderer>().enabled = false;
            //Debug.Log(projectilesPath[i].gameObject.name);
        }
    }

    private void Rotate()
    {
        Vector2 dir = GameObject.Find("dot (1)").transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // 캐논에 박스콜라이더 없어야함. 있으면 반대로 발사 안되고 발사위치도 살짝내려가버림
    private IEnumerator Shoot()
    {
        for (int i = 0; i < gc.ballsCount; i++)
        {
            yield return new WaitForSeconds(0.07f);
            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            ball.name = "Ball";
            ball.transform.SetParent(ballsContainer.transform);
            ballBody = ball.GetComponent<Rigidbody2D>();
            ballBody.AddForce(ShootForce(Input.mousePosition));

            int balls = gc.ballsCount - i;
            gc.ballsCountText.text = (gc.ballsCount - i - 1).ToString();
        }

        yield return new WaitForSeconds(0.5f);
        gc.shotCount++;
        gc.ballsCountText.text = gc.ballsCount.ToString();
    }
}
