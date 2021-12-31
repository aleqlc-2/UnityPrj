using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public AudioClip death;

    // Enemy의 캡슐콜라이더에 isTrigger를 체크하니 조인트가 망가지지않고 고정됨
    // 총알프리펩 콜라이더의 isTrigger를 체크해야 총알이 관통하고 지나감
    void OnTriggerEnter2D(Collider2D target)
    {
        Vector2 direction = transform.position - target.transform.position;

        if (target.tag == "Bullet")
        {
            if (transform.GetChild(0).GetComponent<Rigidbody2D>().gravityScale < 1 && this.gameObject.tag == "Enemy") Death(); // Head

            // 총알이 관통하는 방향대로 쓰러지도록
            GetComponent<Rigidbody2D>().AddForce(new Vector2(direction.x > 0 ? 1 : -1, direction.y > 0 ? 0.3f : -0.3f) * 10f, ForceMode2D.Impulse);
        }

        if (target.tag == "Plank" || target.tag == "BoxPlank")
        {
            if (target.GetComponent<Rigidbody2D>().velocity.magnitude > 1.5f) Death();
        }

        if (target.tag == "Ground")
        {
            if (GetComponent<Rigidbody2D>().velocity.magnitude > 2f) Death();
        }
    }

    private void Death()
    {
        gameObject.tag = "Untagged";

        FindObjectOfType<GameManager>().CheckEnemyCount(); // 적이 죽을때마다 win인지 검사

        SoundManager.instance.PlaySoundFX(death, 0.75f);

        foreach (Transform obj in transform) // 모든 자식개체를 돌게됨
        {
            obj.GetComponent<Rigidbody2D>().gravityScale = 1f;
        }
    }
}
