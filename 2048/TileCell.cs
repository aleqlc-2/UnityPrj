using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; } // 외부에서읽기 외부에서set 모두가능
    public Tile tile { get; set; }
    public bool empty => tile == null; // 이 스크립트의 프로퍼티 중 tile이 null이면 true, null이 아니면 false반환
	public bool occupied => tile != null; // 이 스크립트의 프로퍼티 중 tile이 null이 아니면 true, null이면 false반환
}
