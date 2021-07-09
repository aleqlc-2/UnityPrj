using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float startForce = 15f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * startForce, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Arrow")
        {
            Destroy(this.gameObject);
        }
    }
}
