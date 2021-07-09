using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPiece : MonoBehaviour
{
    public bool isColored = false;

    public void ChangeColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
        isColored = true;

        // 색칠할때마다 다 색칠했는지 검사해서 다 색칠했으면 다음 scene으로
        GameManager.singleton.CheckComplete();
    }
}
