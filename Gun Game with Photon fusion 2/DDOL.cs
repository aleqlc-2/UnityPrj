using UnityEngine;

//GlobalManagers, NetworkRunnerController ������Ʈ�� �θ������Ʈ�� ������ ��ũ��Ʈ
public class DDOL : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(this);
	}
}
