using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class airplane : MonoBehaviour
{
    public GameObject Explosion;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Bullet")
        {
            Vector3 temp = transform.position;
            temp.y += 1f;
            Instantiate(Explosion, temp, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
