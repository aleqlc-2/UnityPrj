using UnityEngine;
using UnityEngine.SceneManagement;

public class Blaster : MonoBehaviour
{
	// MonoBehaviour 부모클래스에 rigidbody 같은이름의 변수가있어서 자식클래스에서 new를 붙여 자식클래스의 지역변수임을 명시화한다
	private new Rigidbody2D rigidbody;

	private Vector2 direction;
	public float speed = 20f;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		Vector2 position = rigidbody.position;
		position += direction.normalized * speed * Time.fixedDeltaTime;
		rigidbody.MovePosition(position);
	}

	private void Update()
	{
		direction.x = Input.GetAxis("Horizontal");
		direction.y = Input.GetAxis("Vertical");
	}

	public void NewGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
