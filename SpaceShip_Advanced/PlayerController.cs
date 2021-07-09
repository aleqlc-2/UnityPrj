using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public enum InputState
	{
		PC,
		MOBILE
	}

	[SerializeField] private InputState inputState = InputState.PC;

	private float speed = 8f;

	[SerializeField] private float min_X = 0, max_X = 0;
	[SerializeField] private float min_Y = 0, max_Y = 0;

	[SerializeField] private GameObject player_Bulle = null;
	[SerializeField] private GameObject bomb_Prefab = null;
	[SerializeField] private Transform attack_Point = null;

	private float attack_Timer = 0.1f;
	private float current_Attack_Timer = 0f;
	private bool canAttack = true;
	private bool isDead = false;
	private bool isBombRemaining = true;

	private Vector3 originPosition = Vector3.one;

	[SerializeField] private GameObject player = null;

	private List<GameObject> bulletList = new List<GameObject>();

	private bool inRespawnTime = false;
	private bool isBlinking = false;

	// 모바일 touchpad input
	//private float deltaX, deltaY;
	//private Rigidbody2D rb = null;
	//private bool moveAllowed = false;

	// 모바일 joyPanel input
	[SerializeField] private bool[] joyControl = Enumerable.Repeat(false, 9).ToArray();
	private bool isControl = false;
	private bool isJoyAttack = false;
	private bool isJoyBomb = false;

	private void Start()
	{
		current_Attack_Timer = attack_Timer;
		originPosition = transform.position;

		//// 모바일 touchpad input
		//rb = GetComponent<Rigidbody2D>();
		//PhysicsMaterial2D mat = new PhysicsMaterial2D();
		//mat.bounciness = 0.75f;
		//mat.friction = 0.4f;
		//GetComponent<CircleCollider2D>().sharedMaterial = mat;
	}

	private void Update()
	{
		switch (inputState)
		{
			case InputState.PC:
				MovePlayer();
				Attack();
				break;
			case InputState.MOBILE:
				JoyMove();
				JoyAttack();
				JoyBombFire();
				break;
		}
	}

	private void MovePlayer()
	{
		if (Input.GetAxisRaw("Vertical") > 0f)
		{
			Vector3 temp = transform.position;
			temp.y += speed * Time.deltaTime;

			if (temp.y > max_Y)
				temp.y = max_Y;

			transform.position = temp;
		}
		else if (Input.GetAxisRaw("Vertical") < 0f)
		{
			Vector3 temp = transform.position;
			temp.y -= speed * Time.deltaTime;

			if (temp.y < min_Y)
				temp.y = min_Y;

			transform.position = temp;
		}

		if (Input.GetAxisRaw("Horizontal") > 0f)
		{
			Vector3 temp = transform.position;
			temp.x += speed * Time.deltaTime;

			if (temp.x > max_X)
				temp.x = max_X;

			transform.position = temp;
		}
		else if (Input.GetAxisRaw("Horizontal") < 0f)
		{
			Vector3 temp = transform.position;
			temp.x -= speed * Time.deltaTime;

			if (temp.x < min_X)
				temp.x = min_X;

			transform.position = temp;
		}
	}

	private void Attack()
	{
		if (isDead == false)
		{
			attack_Timer += Time.deltaTime;
			if (attack_Timer > current_Attack_Timer)
			{
				canAttack = true;
			}
		}

		if (Input.GetKey(KeyCode.Space) && isDead == false)
		{
			if (canAttack)
			{
				canAttack = false;
				attack_Timer = 0f;

				var obj = ObjPoolManager.Instance.GetPlayerBullet();
				Debug.Log($"[PlayerController] : obj.InstanceID : {obj.GetInstanceID()}");
				obj.transform.position = attack_Point.position;
				obj.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
				obj.GetComponent<BulletScript>().is_EnemyBullet = false;

				bulletList.Add(obj);
			}
		}
		else if (Input.GetKeyDown(KeyCode.K) && isDead == false)
		{
			if (isBombRemaining)
			{
				Instantiate(bomb_Prefab, transform.position + new Vector3(0f, 2f, 0f), Quaternion.identity);
				GameObject bombs = GameObject.FindWithTag("Bombs");
				bombs.transform.GetChild(GameManager.Instance.Bombs - 1).gameObject.SetActive(false); // 폭탄 이미지 1개 비활성화
				GameManager.Instance.Bombs--;
			}

			if (GameManager.Instance.Bombs <= 0)
			{
				isBombRemaining = false;
			}
		}
	}

	float prevH;
	float prevV;
	private void JoyMove()
	{
		if (isControl == true)
		{
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");


			if (joyControl[0])
			{
				h = -1; v = 1; prevH = h;
				prevV = v;
			}
			if (joyControl[1])
			{
				h = 0; v = 1; prevH = h;
				prevV = v;
			}
			if (joyControl[2])
			{
				h = 1; v = 1; prevH = h;
				prevV = v;
			}
			if (joyControl[3])
			{
				h = -1; v = 0; prevH = h;
				prevV = v;
			}
			if (joyControl[4]) { h = -prevH; v = -prevV; }
			if (joyControl[5])
			{
				h = 1; v = 0; prevH = h;
				prevV = v;
			}
			if (joyControl[6])
			{
				h = -1; v = -1; prevH = h;
				prevV = v;
			}
			if (joyControl[7])
			{
				h = 0; v = -1; prevH = h;
				prevV = v;
			}
			if (joyControl[8])
			{
				h = 1; v = -1; prevH = h;
				prevV = v;
			}

			Vector3 curPos = transform.position;
			Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
			Vector3 temp = curPos + nextPos;

			if (temp.x >= max_X)
				temp.x = max_X;
			if (temp.y >= max_Y)
				temp.y = max_Y;
			if (temp.x <= min_X)
				temp.x = min_X;
			if (temp.y <= min_Y)
				temp.y = min_Y;

			transform.position = temp;
		}
	}

	private void JoyAttack()
	{
		if (isJoyAttack)
		{
			if (isDead == false)
			{
				attack_Timer += Time.deltaTime;
				if (attack_Timer > current_Attack_Timer)
				{
					canAttack = true;
				}
			}

			if (canAttack)
			{
				canAttack = false;
				attack_Timer = 0f;

				var obj = ObjPoolManager.Instance.GetPlayerBullet();
				Debug.Log($"[PlayerController] : obj.InstanceID : {obj.GetInstanceID()}");
				obj.transform.position = attack_Point.position;
				obj.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
				obj.GetComponent<BulletScript>().is_EnemyBullet = false;

				bulletList.Add(obj);
			}
		}
	}

	private void JoyBombFire()
	{
		if (isJoyBomb && !isDead)
		{
			if (GameManager.Instance.Bombs <= 0)
			{
				isBombRemaining = false;
			}

			if (isBombRemaining)
			{
				Instantiate(bomb_Prefab, transform.position + new Vector3(0f, 2f, 0f), Quaternion.identity);
				GameObject bombs = GameObject.FindWithTag("Bombs");
				bombs.transform.GetChild(GameManager.Instance.Bombs - 1).gameObject.SetActive(false); // 폭탄 이미지 1개 비활성화
				GameManager.Instance.Bombs--;
				isJoyBomb = false;
			}
		}
	}

	public void JoyPanel(int type)
	{
		for (int index = 0; index < 9; index++)
		{
			joyControl[index] = index == type;
		}
	}

	public void JoyDown() // move
	{
		isControl = true;
	}

	public void JoyUp() // stop
	{
		isControl = false;
	}

	public void JoyAttackDown() // 일반공격
	{
		isJoyAttack = true;
	}

	public void JoyAttackUp() // 공격중지
	{
		isJoyAttack = false;
	}

	public void JoyBombDown() // 폭탄공격
	{
		isJoyBomb = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!inRespawnTime)
		{
			if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Bullet")
			{
				inRespawnTime = true;

				player.GetComponent<SpriteRenderer>().enabled = false;
				canAttack = false;
				isDead = true;
				GameManager.Instance.Lives--;
				UIManager.instance.TurnOffHeart();

				StartCoroutine(Reposition());
			}
		}
	}

	private IEnumerator Reposition()
	{
		if (GameManager.Instance.Lives > 0)
		{
			yield return new WaitForSeconds(2f);

			player.GetComponent<SpriteRenderer>().enabled = true;
			canAttack = true;
			isDead = false;
			player.transform.position = originPosition;
			isBlinking = true;
			StartCoroutine(Blink());

			yield return new WaitForSeconds(3f); // 리스폰시 무적시간 3초
			isBlinking = false;
			inRespawnTime = false;
			StopCoroutine(Blink());
		}
		StopCoroutine(Reposition());
	}

	private IEnumerator Blink()
	{
		if (isBlinking)
		{
			float t = 0f;
			while (t <= 0.03f)
			{
				t += Time.deltaTime;
				this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
				yield return new WaitForSeconds(0.2f);
				this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
				yield return new WaitForSeconds(0.2f);
			}
		}
	}

	//private void MobileMoveAndAttack()
	//{
	//	if (Input.touchCount > 0)
	//	{
	//		Touch touch = Input.GetTouch(0);

	//		Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

	//		switch (touch.phase)
	//		{
	//			case TouchPhase.Began:
	//				MobileAttack();
	//				if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos))
	//				{
	//					deltaX = touchPos.x - transform.position.x;
	//					deltaY = touchPos.y - transform.position.y;

	//					moveAllowed = true;

	//					rb.freezeRotation = true;
	//					rb.velocity = new Vector2(0, 0);
	//					rb.gravityScale = 0;

	//					GetComponent<CircleCollider2D>().sharedMaterial = null;
	//				}
	//				break;

	//			case TouchPhase.Moved:
	//				if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos) && moveAllowed)
	//				{
	//					// 좌우로만 움직일경우 touchPos.y - deltaY 대신에 transform.position.y를 넣음
	//					rb.MovePosition(new Vector2(touchPos.x - deltaX, touchPos.y - deltaY));
	//				}
	//				break;

	//			case TouchPhase.Ended:
	//				moveAllowed = false;
	//				rb.freezeRotation = false;
	//				rb.gravityScale = 2;

	//				PhysicsMaterial2D mat = new PhysicsMaterial2D();
	//				mat.bounciness = 0.75f;
	//				mat.friction = 0.4f;
	//				GetComponent<CircleCollider2D>().sharedMaterial = mat;
	//				break;
	//		}
	//	}
	//}

	//private void MobileAttack()
	//{
	//	if (isDead == false)
	//	{
	//		attack_Timer += Time.deltaTime;
	//		if (attack_Timer > current_Attack_Timer)
	//		{
	//			canAttack = true;
	//		}
	//	}

	//	if (isDead == false)
	//	{
	//		if (canAttack)
	//		{
	//			canAttack = false;
	//			attack_Timer = 0f;

	//			var obj = ObjPoolManager.Instance.GetPlayerBullet();
	//			Debug.Log($"[PlayerController] : obj.InstanceID : {obj.GetInstanceID()}");
	//			obj.transform.position = attack_Point.position;
	//			obj.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	//			obj.GetComponent<BulletScript>().is_EnemyBullet = false;

	//			bulletList.Add(obj);
	//		}
	//	}
	//}
}
