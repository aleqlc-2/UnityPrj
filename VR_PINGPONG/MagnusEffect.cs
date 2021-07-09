using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnusEffect : MonoBehaviour
{
    public float MagnusConst = .07f;

    private Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 force = MagnusConst * Vector3.Cross(r.velocity, r.angularVelocity);
        r.AddRelativeForce(force);
    }
}
