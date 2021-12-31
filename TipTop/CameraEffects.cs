using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public Vector2 RotateRange = new Vector2(-3f, 3f);

    public float Delta = 3f;
    public float Delay = 3f;
    public float TargetRot = 15f;
    private float rot;
    private float targetRotation;

    private bool r;

    private void Start()
    {
        StartCoroutine(Rotation());
    }

    private IEnumerator Rotation()
    {
        while (true)
        {
            targetRotation = TargetRot;
            yield return StartCoroutine(Rot());

            targetRotation = 0f;
            yield return StartCoroutine(Rot());

            targetRotation = -TargetRot;
            yield return StartCoroutine(Rot());
        }
    }

    private IEnumerator Rot()
    {
        float t = 0f;
        yield return new WaitForSeconds(Delay);
        while (t < 2f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, targetRotation), 0.05f);
            t += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
