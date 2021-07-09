using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 4f;

    public Rigidbody2D rb;

    private float movement = 0f;

    void FixedUpdate()
    {
        movement = Input.GetAxisRaw("Horizontal") * speed * Time.fixedDeltaTime; // GetAxis는 늦게 멈춤
    }

    void Update()
    {
        rb.MovePosition(rb.position + new Vector2(movement , 0f));
        // rb.MovePosition(rb.position + Vector2.right * movement); //감도가 느림
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Ball")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
