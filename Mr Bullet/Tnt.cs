using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tnt : MonoBehaviour
{
    public GameObject explosionPrefab;

    public float radius = 1f;
    public float power = 5f;

    void OnCollisionEnter2D(Collision2D target)
    {
        if (target.gameObject.tag == "Bullet")
        {
            GameObject exp = Instantiate(explosionPrefab); // Instantiate 메서드에 위치, 회전주면 폭발이 수평으로만 됨
            exp.transform.position = transform.position;
            Explode();
            Destroy(exp, 0.8f);
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        Vector2 explosionPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, radius);

        foreach (Collider2D hit in colliders)
        {
            if (hit.GetComponent<Rigidbody2D>() != null)
            {
                Vector2 explodeDir = hit.GetComponent<Rigidbody2D>().position - explosionPos;

                hit.GetComponent<Rigidbody2D>().gravityScale = 1f;
                hit.GetComponent<Rigidbody2D>().AddForce(power * explodeDir, ForceMode2D.Impulse);
            }

            if (hit.tag == "Enemy") hit.tag = "Untagged";
        }
    }
    
    //// 실행 안해도 그려짐
    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(transform.position, radius);
    //}
}
