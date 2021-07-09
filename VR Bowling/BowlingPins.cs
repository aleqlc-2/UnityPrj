using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// pin들의 부모개체에 들어갈 스크립트
public class BowlingPins : MonoBehaviour
{
    private List<Vector3> startPos = new List<Vector3>();
    private List<Transform> pins = new List<Transform>();

    private int numPinsDown = 0;

    void Start()
    {
        // 모든 pin들의 transform을 불러와서
        foreach (Transform child in transform) // 이런식으로 부르면 부모개체는 안들어가는듯
        {
            startPos.Add(child.position); // position은 Vector3 자료형으로 전부 저장하고
            pins.Add(child); // transform(position, rotation, scale)은 Transform 자료형으로 전부 저장한다
        }
    }

    void Update()
    {
        // 모든 pin들의 transform을 불러와서
        foreach(Transform child in transform)
        {
            // pin이 아래로 떨어지면
            if (child.gameObject.activeInHierarchy && child.position.y < -5f)
            {
                child.gameObject.SetActive(false); // 그 pin은 비활성화, Destroy한건 아님.
                numPinsDown++;
            }
        }

        // GetComponentsInChildren 메서드로 가져오지 않았으니 부모개체는 안들어가는듯
        if (numPinsDown == pins.Count) // 모든 pin이 아래로 떨어지면
        {
            Reset();
        }
    }

    void Reset()
    {
        for (int i = 0; i < pins.Count; i++)
        {
            pins[i].gameObject.SetActive(true);
            pins[i].transform.position = startPos[i];
            pins[i].rotation = Quaternion.identity;
            Rigidbody r = pins[i].GetComponent<Rigidbody>();
            r.velocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
        }
        numPinsDown = 0;
    }
}
