using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBullet : MonoBehaviour
{
    [SerializeField] private Transform bigBulletPoint = null;
    private float speed = 20f;

    private bool isfire = false;
    public bool isFire
	{
		get
		{
			return isfire;
		}
		set
		{
            isfire = value;
		}
	}

    //private void Start()
    //{
    //    isfire = false;
    //}

    private void Update()
    {
        if (isfire)
        {
            transform.localScale = transform.localScale + Vector3.down * Time.deltaTime * speed;
            StartCoroutine(BigBulletReset());
        }
    }

    private IEnumerator BigBulletReset()
    {
        yield return new WaitForSeconds(1.5f);
        isfire = false;
        transform.position = bigBulletPoint.position;
        transform.localScale = new Vector3(1f, 0f, 1f);
    }
}
