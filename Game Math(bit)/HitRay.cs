using UnityEngine;

public class HitRay : MonoBehaviour
{
	// ~붙이면 다음 레이어가됨
	//private void Start()
	//{
	//	int i = 0;
	//	i = ~i;
	//	Debug.Log(i);

	//	int j = 1;
	//	j = ~j;
	//	Debug.Log(j);

	//	int a = 2;
	//	a = ~a;
	//	Debug.Log(a);

	//	int b = 3;
	//	b = ~b;
	//	Debug.Log(b);
	//}

	void Update()
    {
        int layerMask = (1 << 7) | (1 << 9); // 2진수로 or연산해서 에디터상 7번이나 9번의 layermask를 인식하도록
		RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
			// hit.distance곱해서 hit된 물체까지만 그리도록
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Hit");
        }
        else
        {
			// hit과 상관없이 1000만큼 그림
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.red);
			Debug.Log("Missed");
		}
    }
}
