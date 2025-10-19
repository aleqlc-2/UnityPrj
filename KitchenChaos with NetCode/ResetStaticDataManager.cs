using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
	// GameScene에서 static 이벤트들의 Invoke리스트가 누적되어있으므로 메인메뉴갈때 초기화
	private void Awake()
	{
		CuttingCounter.ResetStaticData();
		BaseCounter.ResetStaticData();
		TrashCounter.ResetStaticData();
		Player.ResetStaticData();
	}
}
