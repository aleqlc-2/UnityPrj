using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviour 안지우면 이 클래스를 객체화 한 Matrix[row, column]가 항상 null 뜨므로 지워줘야함
// 프로퍼티로만 쓸 클래스들은 지워주는게 좋을듯
public class PuzzlePiece
{
    public int CurrentRow { get; set; }
    public int CurrentColumn { get; set; }

    public int OriginalRow { get; set; }
    public int OriginalColumn { get; set; }

    public GameObject obj { get; set; }
}
