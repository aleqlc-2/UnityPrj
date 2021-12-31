using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisObject : MonoBehaviour
{
    private float lastFall = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // 좌이동
        {
            transform.position += new Vector3(-1, 0, 0);

            if (IsValidGridPosition()) // 유효하면
            {
                UpdateMatrixGrid(); // update
            }
            else
            {
                transform.position += new Vector3(1, 0, 0); // 원위치로
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) // 우이동
        {
            transform.position += new Vector3(1, 0, 0);

            if (IsValidGridPosition()) // 유효하면
            {
                UpdateMatrixGrid(); // update
            }
            else // 유효하지 않으면
            {
                transform.position += new Vector3(-1, 0, 0); // 원위치로
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) // 회전
        {
            transform.Rotate(new Vector3(0, 0, -90));

            if (IsValidGridPosition()) // 유효하면
            {
                UpdateMatrixGrid(); // update
            }
            else // 유효하지 않으면
            {
                transform.Rotate(new Vector3(0, 0, 90)); // 원래회전값으로
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= 1) // 아래키 누름 또는 1초마다 조각 내려옴, ==왜안되지
        {
            transform.position += new Vector3(0, -1, 0);
            
            if (IsValidGridPosition()) // 유효하면
            {
                UpdateMatrixGrid(); // update
            }
            else // 유효하지 않으면(더 이상 내려갈 수 없으면)
            {
                transform.position += new Vector3(0, 1, 0); // 원위치로

                MatrixGrid.DeleteWholeRows(); // 줄 삭제해야하는지 체크

                enabled = false; // 이 조각에 부착된 스크립트를 비활성화하여 더이상 input의 영향을 받지 않도록

                FindObjectOfType<Spawner>().SpawnRandom(); // 새로운 조각 생성
            }

            lastFall = Time.time; // 이 코드 없으면 조각들 엄청빨리 내려와버림
        }
    }

    private bool IsValidGridPosition()
    {
        foreach (Transform child in transform) // 이 스크립트가 부착된 오브젝트의 자식개체들을 전부 불러와서
        {
            Vector2 v = MatrixGrid.RoundVector(child.position);

            if (!MatrixGrid.IsInsideBorder(v)) return false;

            if (MatrixGrid.grid[(int)v.x, (int)v.y] != null && MatrixGrid.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }

        return true;
    }

    private void UpdateMatrixGrid()
    {
        // 모든 grid에 null 할당
        for (int y = 0; y < MatrixGrid.column; ++y) // ++y
        {
            for (int x = 0; x < MatrixGrid.row; ++x) // ++x
            {
                if (MatrixGrid.grid[x, y] != null)
                {
                    // MatrixGrid.grid[x, y]가 이 스크립트가 부착된 오브젝트의 자식개체라면
                    if (MatrixGrid.grid[x, y].parent == transform)
                    {
                        MatrixGrid.grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform child in transform)
        {
            Vector2 v = MatrixGrid.RoundVector(child.position);
            MatrixGrid.grid[(int)v.x, (int)v.y] = child;
        }
    }
}
