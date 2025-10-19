using UnityEngine;

public class TileRow : MonoBehaviour
{
    public TileCell[] cells { get; private set; } // 외부에서 읽기가능, 내부에서만 set

	private void Awake()
	{
		cells = GetComponentsInChildren<TileCell>(); // 이 스크립트가 부착된 오브젝트의 자식개체들 모두에게서 TileCell스크립트를 추출
	}
}
