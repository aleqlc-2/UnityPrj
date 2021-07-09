using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private float speed = 5f;
    private float rotate_Speed = 50f;
    private float bound_Y = -11f;

    [SerializeField] private bool canShoot = false;
    [SerializeField] private bool canRotate = false;
    private bool canMove = true;

    [SerializeField] private Transform attack_Point = null;
    [SerializeField] private GameObject bullet_Prefab = null;

    private int hitCount = 0;
    public int HitCount
	{
		get { return hitCount; }
        set { hitCount = value; }
	}

	private void OnEnable()
	{
        ResetRandomPos();
        StartCoroutine(StartShooting());
    }

	private void Start()
    {
        if (canRotate)
        {
            speed = 10f;

            if (Random.Range(0, 2) > 0)
            {
                rotate_Speed = Random.Range(rotate_Speed, rotate_Speed + 20f);
                rotate_Speed *= -1f;
            }
            else
            {
                rotate_Speed = Random.Range(rotate_Speed, rotate_Speed + 20f);
            }
        }
	}

    private void Update()
    {
        Move();
        RotateEnemy();
    }

    private void Move()
    {
        if (canMove)
        {
            Vector3 temp = transform.position;
            temp.y -= speed * Time.deltaTime;
            transform.position = temp;

            if (temp.y < bound_Y)
                gameObject.SetActive(false);
        }
    }

    private void ResetRandomPos()
	{
        float pos_X = Random.Range(-4f, 4f);
        transform.position = new Vector3(0f, 10f, 0f);
        Vector3 temp = transform.position;
        temp.z = -1;
        temp.x = pos_X;
        transform.position = temp;
    }

    private void RotateEnemy()
    {
        if (canRotate)
        {
            transform.Rotate(new Vector3(0f, 0f, rotate_Speed * Time.deltaTime), Space.World);
        }
    }

    private IEnumerator StartShooting()
    {
        yield return null; // 적비행기 생성후 비활성화 되기전 발사하는 부분 없애기위해
        while(canShoot)
		{
             GameObject bullet = Instantiate(bullet_Prefab, attack_Point.position, Quaternion.Euler(0f, 0f, 90f));
             bullet.GetComponent<BulletScript>().is_EnemyBullet = true;

             yield return new WaitForSeconds(Random.Range(2f, 4f));
        }
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "Bullet")
        {
            // stage1 enemy
            if (!canShoot)
            {
                StartCoroutine(TurnOffGameObject());
            }

            // stage2 enemy
            if (canShoot) hitCount++;
            if (canShoot && hitCount == 3)
            {
                StopCoroutine(StartShooting());
                StopCoroutine(TurnOffGameObject());
                StartCoroutine(TurnOffGameObject());
            }
		}
    }

    private IEnumerator TurnOffGameObject()
    {
        yield return new WaitForSeconds(0.01f);

		if (!canShoot)
		{
            ObjPoolManager.Instance.ReturnEnemyStage1(gameObject);
		}
		else if (canShoot && hitCount == 3)
		{
            hitCount = 0;
            ObjPoolManager.Instance.ReturnEnemyStage2(gameObject);
        }
	}
}
