using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rocket : MonoBehaviour
{
    private Rigidbody r;
    new private GameObject camera;

    private bool closeToPlayer = false;

    void Start()
    {
        r = GetComponent<Rigidbody>();
        camera = GameObject.FindGameObjectWithTag("MainCamera"); // CameraRig의 Camera

        // 이것도 normalized 안해야하는거 아닌가?
        r.velocity = (camera.transform.position - transform.position).normalized * Random.Range(20f, 30f);
    }

    void FixedUpdate()
    {
        Vector3 velocity = r.velocity;

        // normalized 안하면 그냥 distance, normalized 하면 방향인듯. 그리고 둘다 Vector3
        Vector3 currDist = (camera.transform.position - transform.position);

        if (currDist.magnitude < 0.5f) // 벡터의 길이, float
        {
            closeToPlayer = true;
        }

        if (closeToPlayer)
        {
            // 이것도 normalized 안해야하는거 아닌가?
            r.velocity = (camera.transform.position - transform.position).normalized * velocity.magnitude;
        }
        
        transform.rotation = Quaternion.LookRotation(transform.position + velocity);
    }
}
