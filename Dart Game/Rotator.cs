using UnityEngine;

public class Rotator : MonoBehaviour
{
    public static float speed = 100f;

    void Update()
    {
        transform.Rotate(0f, 0f, Time.deltaTime * speed);
    }
}
