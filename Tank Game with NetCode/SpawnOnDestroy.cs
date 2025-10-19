using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour // client총알 프리펩에 붙은 스크립트
{
    [SerializeField] private GameObject prefab; // 파티클

	// 총알 사라질때
	private void OnDestroy()
	{
		if (!gameObject.scene.isLoaded) return; // 총알오브젝트가 속해있는 scene이 로드되지않았으면 리턴

		Instantiate(prefab, transform.position, Quaternion.identity); // 파티클 생성
	}
}
