using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public float power = 0.7f;
    [HideInInspector] public float shake_Duration = 1f;
    public float shake_SlowdownAmount = 1f;
    private bool should_Shake;

    private Vector3 startPosition;
    private float initialDuration;

    void Update()
    {
        if (should_Shake)
        {
            if (shake_Duration > 0f)
            {
                transform.localPosition = startPosition + Random.insideUnitSphere * power;
                shake_Duration -= Time.unscaledDeltaTime * shake_SlowdownAmount;
            }
            else
            {
                should_Shake = false;
                shake_Duration = initialDuration;
                transform.localPosition = startPosition;
            }
        }
    }

    public void InitializeValues(float duration)
    {
        shake_Duration = duration;
        initialDuration = shake_Duration;

        startPosition = transform.localPosition;
        
        should_Shake = true;
    }
}
