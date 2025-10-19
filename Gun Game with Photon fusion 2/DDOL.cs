using UnityEngine;

//GlobalManagers, NetworkRunnerController 오브젝트의 부모오브젝트에 부착된 스크립트
public class DDOL : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(this);
	}
}
