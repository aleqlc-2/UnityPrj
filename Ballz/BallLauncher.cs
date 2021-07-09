using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    private Vector3 startDragPosition;
    private Vector3 endPosition;
    private LaunchPreview launchPreview;
    private List<Ball> balls = new List<Ball>();
    private int ballsReady;

    [SerializeField]
    private Ball ballPrefab;

    private BlockSpawner blockSpawner;

    private void Awake()
    {
        blockSpawner = FindObjectOfType<BlockSpawner>();
        launchPreview = GetComponent<LaunchPreview>();
        CreateBall();
    }

    public void ReturnBall()
    {
        ballsReady++;
        if (ballsReady == balls.Count) // ball이 다 돌아왔으면
        {
            blockSpawner.SpawnRowOfBlocks();
            CreateBall();
        }
    }

    private void CreateBall()
    {
        var ball = Instantiate(ballPrefab);
        balls.Add(ball);
        ballsReady++;
    }

    private void Update()
    {
        // Vector3.back 이 z축으로 -1이고 -10곱했으므로 10이 되어 내쪽에서 10만큼 멀어지는것.
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.back * -10;

        if (Input.GetMouseButtonDown(0))
        {
            StartDrag(worldPosition);
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueDrag(worldPosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void StartDrag(Vector3 worldPosition)
    {
        startDragPosition = worldPosition;

        launchPreview.SetStartPoint(transform.position);
    }

    private void ContinueDrag(Vector3 worldPosition)
    {
        endPosition = worldPosition;

        Vector3 direction = endPosition - startDragPosition;
        launchPreview.SetEndPoint(transform.position - direction); // 더하면 내가 끄는대로 따라옴
    }

    private void EndDrag()
    {
        StartCoroutine(LaunchBalls());
    }

    private IEnumerator LaunchBalls()
    {
        Vector3 direction = endPosition - startDragPosition;
        direction.Normalize();

        foreach (var ball in balls)
        {
            ball.transform.position = transform.position;
            ball.gameObject.SetActive(true);
            ball.GetComponent<Rigidbody2D>().AddForce(-direction);

            yield return new WaitForSeconds(0.1f);
        }
        ballsReady = 0;
    }
}
