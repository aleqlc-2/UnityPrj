using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBullet : MonoBehaviour
{
	[SerializeField] private Transform rotateSpawnPoint = null;
	[SerializeField] private GameObject rotateBullet = null;

	private List<GameObject> objList = new List<GameObject>();

	private float min_Z = -45F, max_Z = 45f;
	private float rotate_Speed = 50f;
	private float rotate_Angle = 0f;
	private bool rotate_Right = false;
	private bool isRotateBullet = false;
	public bool IsRotateBullet
	{
		get
		{
			return isRotateBullet;
		}
		set
		{
			isRotateBullet = value;
		}
	}

	void OnEnable() // 활성화될때마다 rotateBullet 생성하기위해
	{
		StartCoroutine(ShootBullet());
	}

	void Update()
	{
		if (rotate_Right)
			rotate_Angle += rotate_Speed * Time.deltaTime;
		else
			rotate_Angle -= rotate_Speed * Time.deltaTime;

		rotateSpawnPoint.rotation = Quaternion.AngleAxis(rotate_Angle, Vector3.forward);

		if (rotate_Angle >= max_Z)
			rotate_Right = false;
		else if (rotate_Angle <= min_Z)
			rotate_Right = true;
	}

	private IEnumerator ShootBullet()
	{
		isRotateBullet = true;
		for (int i = 0; i < 30; i++)
		{
			yield return new WaitForSeconds(0.2f);
			var obj = ObjPoolManager.Instance.GetRotateBullet();
			obj.transform.position = rotateSpawnPoint.position;
			obj.transform.rotation = rotateSpawnPoint.rotation;
			objList.Add(obj);
		}
		isRotateBullet = false;
		yield return new WaitForSeconds(2f);

		ObjPoolManager.Instance.ReturnRotateBullet(objList);
	}
}