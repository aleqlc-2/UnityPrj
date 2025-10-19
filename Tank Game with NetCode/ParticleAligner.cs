using UnityEngine;

// track��ƼŬ�� ������ ��ũ��Ʈ
[RequireComponent(typeof(ParticleSystem))]
public class ParticleAligner : MonoBehaviour
{
    private ParticleSystem.MainModule psMain;

    void Start()
    {
		psMain = GetComponent<ParticleSystem>().main;

	}

    void Update()
    {
		// �Ϲݰ����� Mathf.Deg2Rad�� ���ϸ� ������ ��
		// track ��ƼŬ�� ���������� ���Ľ����ش�
		psMain.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
	}
}
