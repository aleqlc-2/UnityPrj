using UnityEngine;

public class HitRay : MonoBehaviour
{
	// ~���̸� ���� ���̾��
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
        int layerMask = (1 << 7) | (1 << 9); // 2������ or�����ؼ� �����ͻ� 7���̳� 9���� layermask�� �ν��ϵ���
		RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
			// hit.distance���ؼ� hit�� ��ü������ �׸�����
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Hit");
        }
        else
        {
			// hit�� ������� 1000��ŭ �׸�
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.red);
			Debug.Log("Missed");
		}
    }
}
