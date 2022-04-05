using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMovementMotor : MovementMotor
{
    public float walkingSpeed = 5f;
    public float walkingSnappyness = 50f;
    public float turningSmoothing = 0.3f;

    private Rigidbody myBody;

    void Awake()
    {
        myBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 targetVelocity = movementDirection * walkingSpeed;
        Vector3 deltaVelocity = targetVelocity - myBody.velocity;

        if (myBody.useGravity) deltaVelocity.y = 0f;

        myBody.AddForce(deltaVelocity * walkingSnappyness, ForceMode.Acceleration);

        Vector3 faceDir = facingDirection;

        if (faceDir == Vector3.zero)
        {
            myBody.angularVelocity = Vector3.zero;
        }
        else
        {
            float rotationAngle = AngleAroundAxis(transform.forward, faceDir, Vector3.up);
            myBody.angularVelocity = (Vector3.up * rotationAngle * turningSmoothing);
        }
    }

    private float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        dirA = dirA - Vector3.Project(dirA, dirB);
        dirB = dirB - Vector3.Project(dirB, dirA);

        float angle = Vector3.Angle(dirA, dirB);

        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }
}
