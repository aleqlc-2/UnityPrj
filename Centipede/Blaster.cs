using UnityEngine;
using UnityEngine.SceneManagement;

public class Blaster : MonoBehaviour
{
	// MonoBehaviour �θ�Ŭ������ rigidbody �����̸��� �������־ �ڽ�Ŭ�������� new�� �ٿ� �ڽ�Ŭ������ ������������ ���ȭ�Ѵ�
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
