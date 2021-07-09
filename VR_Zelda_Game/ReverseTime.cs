using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseTime : MonoBehaviour
{
    private List<Vector3> positions = new List<Vector3>();
    private List<Quaternion> rotations = new List<Quaternion>();

    private bool recording = false;

    void Update()
    {
        if (recording)
        {
            positions.Add(transform.position);
            rotations.Add(transform.rotation);
        }
    }

    public void Record()
    {
        recording = true;
    }

    public IEnumerator Playback(int speed) // public
    {
        Rigidbody r = GetComponent<Rigidbody>();
        r.isKinematic = true; // 다시 날아올때는 물리효과 미적용(날아오다가 서로 부딪히지않도록)
        recording = false;

        for (int i = positions.Count - 1; i >= 0; i -= speed)
        {
            transform.position = positions[i];
            transform.rotation = rotations[i];
            yield return null; // 1개값 되감고 1프레임쉬고 반복
        }

        r.isKinematic = false; // 다 날아오고나면 물리효과 다시 적용
        Reset();
    }

    public void Reset()
    {
        positions.Clear();
        rotations.Clear();
    }
}
