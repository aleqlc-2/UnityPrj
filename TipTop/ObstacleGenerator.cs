using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public static Queue<GameObject> Obstacles;

    public int poolSize = 50;

    public float speed = 10f;
    public float smooth = 5f;
    private float topHeight, topWidth;
    private float bottomHeight, bottomWidth;

    public Vector2 WidthRange = new Vector2(3f, 3f);
    public Vector2 HeightRange = new Vector2(2.3f, 4.3f);

    public Transform ObstacleContainer;

    public GameObject Obstacle;
    private GameObject top, bottom;

    private Vector3 startPos;

    private float topInterval
    {
        get => (topWidth - smooth / speed) / speed; // return
    }

    private float bottomInterval
    {
        get => (bottomWidth - smooth / speed) / speed;
    }

    private Vector3 topScale
    {
        get => new Vector3(topWidth, topHeight, 1);
    }

    private Vector3 bottomScale
    {
        get => new Vector3(bottomWidth, bottomHeight, 1f);
    }

    private void Awake()
    {
        startPos = new Vector3(15f, 0f, 0f);
        FillPool();
    }

    private void FillPool()
    {
        Obstacles = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject clone = Instantiate(Obstacle, startPos, Quaternion.identity, ObstacleContainer);
            clone.SetActive(false);
            Obstacles.Enqueue(clone);
        }
    }

    private void Start()
    {
        StartCoroutine(TopRandGen());
        StartCoroutine(BottomRandGen());
    }

    private IEnumerator TopRandGen()
    {
        topWidth = WidthRange.x;

        while (true)
        {
            top = GetObstacle();
            topHeight = Random.Range(HeightRange.x, HeightRange.y);
            UpdateTopTransform();
            yield return new WaitForSeconds(topInterval);
            top.SetActive(true);
        }
    }

    private IEnumerator BottomRandGen()
    {
        bottomWidth = WidthRange.x;

        while (true)
        {
            bottom = GetObstacle();
            bottomHeight = Random.Range(HeightRange.x, HeightRange.y);
            UpdateBottomTransform();
            yield return new WaitForSeconds(bottomInterval);
            bottom.SetActive(true);
        }
    }

    private GameObject GetObstacle()
    {
        GameObject clone = Obstacles.Dequeue();
        clone.transform.position = startPos;
        UpdateSpeed();
        return clone;
    }

    private void UpdateSpeed()
    {
        ObstacleMover.Speed = speed;
    }

    private void UpdateTopTransform()
    {
        top.transform.localScale = topScale;
        top.transform.position = new Vector3(top.transform.position.x, 5 - top.transform.localScale.y / 2f, 0f);
    }

    private void UpdateBottomTransform()
    {
        bottom.transform.localScale = bottomScale;
        bottom.transform.position = new Vector3(bottom.transform.position.x, -5 + bottom.transform.localScale.y / 2f, 0f);
    }
} // class















