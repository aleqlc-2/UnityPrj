using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오브젝트에 부착될 스크립트가 아니므로 MonoBehaviour상속받지 않아도됨
public class MatrixGrid// : MonoBehaviour
{
    public static int row = 10;
    public static int column = 20;

    public static Transform[,] grid = new Transform[row, column];

    public static Vector2 RoundVector(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    public static bool IsInsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < row && (int)pos.y >= 0);
    }

    // 한 줄 삭제하는 메서드
    public static void DeleteRow(int y)
    {
        for (int x = 0; x < row; ++x) // ++x
        {
            GameObject.Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    // 한줄 내려오게하는 메서드
    public static void DecreaseRow(int y)
    {
        for (int x = 0; x < row; ++x) // ++x
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0); // 실제로 위치를 낮춤
            }
        }
    }

    // 내려오게 되는 줄의 위의 줄은 전부 한칸씩 내리는 메서드
    public static void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < column; ++i) // ++i
        {
            DecreaseRow(i);
        }
    }

    // 한 줄이 꽉차서 삭제해야하는지 판단하는 메서드
    public static bool IsRowFull(int y)
    {
        for (int x = 0; x < row; ++x) // ++x
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    public static void DeleteWholeRows()
    {
        for (int y = 0; y < column; ++y) // ++y
        {
            if (IsRowFull(y)) // 줄이 꽉찼으면
            {
                DeleteRow(y); // 그 줄 삭제하고
                DecreaseRowsAbove(y + 1); // 그 위줄 모두를 한칸씩 내림
                --y; // 한줄 사라졌으므로 변수도 -1 해줌
            }
        }
    }
}
