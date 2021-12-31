using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixGrid : MonoBehaviour
{
    // 다른 스크립트 객체를 2차원 배열로 선언 + static
    public static MineField[, ] mineFields;

    public static void ShowAllMines()
    {
        foreach (MineField mf in mineFields)
        {
            mf.ShowMine();
        }
    }

    public static bool MineAtCoordinates(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < GameManager.instance.rows && y < GameManager.instance.columns)
        {
            if (mineFields[x, y].isMine)
            {
                return true;
            }
        }

        return false;
    }

    public static int NearMines(int x, int y)
    {
        int count = 0;

        if (MineAtCoordinates(x, y + 1)) // top
            count++;

        if (MineAtCoordinates(x + 1, y + 1)) // top right
            count++;

        if (MineAtCoordinates(x - 1, y + 1)) // top left
            count++;

        if (MineAtCoordinates(x + 1, y)) // right
            count++;

        if (MineAtCoordinates(x - 1, y)) // left
            count++;

        if (MineAtCoordinates(x, y - 1)) // bottom
            count++;

        if (MineAtCoordinates(x + 1, y - 1)) // bottom right
            count++;

        if (MineAtCoordinates(x - 1, y - 1)) // bottom left
            count++;

        return count;
    }

    public static void InvestigateMines(int x, int y, bool[, ] visited)
    {
        if (x >= 0 && y >= 0 && x < GameManager.instance.rows && y < GameManager.instance.columns)
        {
            if (visited[x, y]) return; // 주변 숫자로 가서 검사할때 이전에 검사했던 것은 다시할 필요 없으므로

            mineFields[x, y].ShowMine(); // 클릭한 숫자가 지뢰인지 판별하고 주변에 지뢰가 존재하면 if문에서 return
            mineFields[x, y].ShowNearMinesCount(NearMines(x, y));

            if (NearMines(x, y) > 0) return; // 클릭한 숫자 주변에 지뢰가 하나라도 있으면 숫자만 보여주고 재귀종료

            visited[x, y] = true;

            InvestigateMines(x - 1, y, visited);
            InvestigateMines(x + 1, y, visited);
            InvestigateMines(x - 1, y + 1, visited);
            InvestigateMines(x + 1, y + 1, visited);
            InvestigateMines(x + 1, y - 1, visited);
            InvestigateMines(x - 1, y - 1, visited);
            InvestigateMines(x, y + 1, visited);
            InvestigateMines(x, y - 1, visited);
        }
    }

    public static bool IsGameFinished()
    {
        foreach (MineField mf in mineFields)
        {
            if (mf.IsClicked() && !mf.isMine)
                return false;
        }

        return true;
    }
}
