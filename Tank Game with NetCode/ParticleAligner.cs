using UnityEngine;

// track파티클에 부착된 스크립트
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
		// 일반각도에 Mathf.Deg2Rad를 곱하면 라디안이 됨
		// track 파티클을 일직선으로 정렬시켜준다
		psMain.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
	}
}
