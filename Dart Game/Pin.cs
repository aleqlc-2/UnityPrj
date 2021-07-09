using UnityEngine;

public class Pin : MonoBehaviour
{
    private bool isPinned = false;

    public float speed = 20f;
    public Rigidbody2D rb;

    void Update()
    {
        if (!isPinned)
            rb.MovePosition(rb.position + Vector2.up * Time.deltaTime * speed);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Rotator")
        {
            transform.SetParent(col.transform); // pin을 Rotator의 자식개체로

            // col.GetComponent<Rotator>().speed += 50f; // 다트맞추면 회전빨라지게. 안됨..
            // col.GetComponent<Rotator>().speed *= -1;  // 다트맞추면 회전반대로. 안됨..
            // static붙이고 아래처럼 하니까 됨.
            Rotator.speed += 50f;
            Rotator.speed *= -1;

            Score.PinCount++;
            isPinned = true; // 맞추면 더이상 위로 올라가지 않도록
        }
        else if (col.tag == "Pin")
        {
            FindObjectOfType<GameManager>().EndGame();
        }
    }
}