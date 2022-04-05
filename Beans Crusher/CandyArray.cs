using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CandyArray
{
    GameObject[,] candies = new GameObject[GameVariables.Rows, GameVariables.Columns];

    private GameObject backup1;
    private GameObject backup2;

    // candies배열의 값 중에서 인자로받은 row,column에 해당하는 값을 get,set하는 인덱서
    public GameObject this[int row, int column]
    {
        get
        {
            try
            {
                return candies[row, column];
            }
            catch(Exception e)
            {
                throw;
            }
        }

        set
        {
            candies[row, column] = value;
        }
    }

    public void Swap(GameObject g1, GameObject g2)
    {
        backup1 = g1;
        backup2 = g2;

        var g1Candy = g1.GetComponent<Candy>();
        var g2Candy = g2.GetComponent<Candy>();

        int g1Row = g1Candy.Row;
        int g1Column = g1Candy.Column;
        int g2Row = g2Candy.Row;
        int g2Column = g2Candy.Column;

        // 캔디 배열에서 게임오브젝트를 바꾸는거고
        var temp = candies[g1Row, g1Column];
        candies[g1Row, g1Column] = candies[g2Row, g2Column];
        candies[g2Row, g2Column] = temp;

        // 캔디 자체에 부착된 스크립트의 행,열 값을 바꾸는것
        Candy.SwapRowColumn(g1Candy, g2Candy);
    }

    public void UndoSwap()
    {
        Swap(backup1, backup2);
    }

    public MatchesInfo GetMatches(GameObject go)
    {
        MatchesInfo matchesInfo = new MatchesInfo();

        var horizontalMatches = GetMatchesHorizontally(go);

        // 매치된 캔디들 중에 전체줄 삭제하는 보너스캔디 포함되어있으면
        if (ContainsDestroyWholeRowColumnBonus(horizontalMatches))
        {
            horizontalMatches = GetEntireRow(go); // 그 캔디가 포함된 줄의 모든 캔디(캔디모양에 상관없이) 가져와서

            if (!BonusTypeChecker.ContainsDestroyWholeRowColumn(matchesInfo.BonusesContained))
            {
                matchesInfo.BonusesContained = BonusType.DestroyWholeRowColumn;
            }
        }

        matchesInfo.AddObjectRange(horizontalMatches); // 매치된 캔디리스트로 넣음

        var verticalMatches = GetMatchesVertically(go);

        if (ContainsDestroyWholeRowColumnBonus(verticalMatches))
        {
            verticalMatches = GetEntireColumn(go);

            if (!BonusTypeChecker.ContainsDestroyWholeRowColumn(matchesInfo.BonusesContained))
            {
                matchesInfo.BonusesContained = BonusType.DestroyWholeRowColumn;
            }
        }

        matchesInfo.AddObjectRange(verticalMatches); // 매치된 캔디리스트로 넣음

        return matchesInfo;
    }

    public IEnumerable<GameObject> GetMatches(IEnumerable<GameObject> gos)
    {
        List<GameObject> matches = new List<GameObject>();

        foreach (var go in gos)
        {
            matches.AddRange(GetMatches(go).MatchedCandy); // AddRange는 여러 요소를 한번에 리스트에 추가
        }

        return matches.Distinct();
    }

    private IEnumerable<GameObject> GetMatchesHorizontally(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);

        var candy = go.GetComponent<Candy>();

        // search left
        if (candy.Column != 0) // 제일 앞의 캔디면 왼쪽검색 안하도록
        {
            for (int column = candy.Column - 1; column >= 0; column--)
            {
                if (candies[candy.Row, column].GetComponent<Candy>().IsSameType(candy))
                    matches.Add(candies[candy.Row, column]);
                else // 다른 캔디가 검색되는순간 바로 종료
                    break;
            }
        }

        // search right
        if (candy.Column != GameVariables.Columns - 1) // 제일 마지막 캔디면 오른쪽검색 안하도록
        {
            for (int column = candy.Column + 1; column < GameVariables.Columns; column++)
            {
                if (candies[candy.Row, column].GetComponent<Candy>().IsSameType(candy))
                    matches.Add(candies[candy.Row, column]);
                else // 다른 캔디가 검색되는순간 바로 종료
                    break;
            }
        }

        // 같은종류 캔디가 3개일때부터 매치인정
        if (matches.Count < GameVariables.MinimumMatches) matches.Clear();

        // 리스트에서 중복제거 후 한개의 캔디만 리턴
        return matches.Distinct();
    }

    private IEnumerable<GameObject> GetMatchesVertically(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);

        var candy = go.GetComponent<Candy>();

        if (candy.Row != 0)
        {
            for (int row = candy.Row - 1; row >= 0; row--)
            {
                if (candies[row, candy.Column].GetComponent<Candy>().IsSameType(candy))
                    matches.Add(candies[row, candy.Column]);
                else
                    break;
            }
        }

        if (candy.Row != GameVariables.Rows - 1)
        {
            for (int row = candy.Row + 1; row < GameVariables.Columns; row++)
            {
                if (candies[row, candy.Column].GetComponent<Candy>().IsSameType(candy))
                    matches.Add(candies[row, candy.Column]);
                else
                    break;
            }
        }

        // 같은종류 캔디가 3개일때부터 매치인정
        if (matches.Count < GameVariables.MinimumMatches) matches.Clear();

        // 중복제거 한채로 리턴
        // matches리스트 자체가 중복제거 된 것은 아님
        // matches.Distinct().ToList() 해서 다른 리스트에 담으면 중복제거 된채로 담김
        return matches.Distinct();
    }

    private bool ContainsDestroyWholeRowColumnBonus(IEnumerable<GameObject> matches)
    {
        if (matches.Count() >= GameVariables.MinimumMatches) // IEnumerable<>의 Count는 ()해야함
        {
            foreach (var item in matches)
            {
                if (BonusTypeChecker.ContainsDestroyWholeRowColumn(item.GetComponent<Candy>().Bonus))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // 인자로 던져진 캔디가 해당하는 Row의 모든 캔디를 리스트에 담아서 반환하는 메서드
    // 리스트의 값들을 반복해서 내뱉을 수 있도록 IEnumerable<GameObject>로 반환
    private IEnumerable<GameObject> GetEntireRow(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        int row = go.GetComponent<Candy>().Row;

        for (int column = 0; column < GameVariables.Columns; column++)
        {
            matches.Add(candies[row, column]);
        }

        return matches;
    }

    private IEnumerable<GameObject> GetEntireColumn(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        int column = go.GetComponent<Candy>().Column;

        for (int row = 0; row < GameVariables.Rows; row++)
        {
            matches.Add(candies[row, column]);
        }

        return matches;
    }

    public void Remove(GameObject item)
    {
        candies[item.GetComponent<Candy>().Row, item.GetComponent<Candy>().Column] = null;
    }

    // IEnumerable<> 자료형의 인자면 반복추출가능한 리스트임
    public AlteredCandyInfo Collapse(IEnumerable<int> columns)
    {
        AlteredCandyInfo collapseInfo = new AlteredCandyInfo();

        foreach (var column in columns) // 모든 열과
        {
            for (int row = 0; row < GameVariables.Rows - 1; row++) // 행을 다 돌면서
            {
                if (candies[row, column] == null) // 비어있는 곳이 있으면
                {
                    for (int row2 = row + 1; row2 < GameVariables.Rows; row2++)
                    {
                        if (candies[row2, column] != null) // 바로 위에 캔디가 있으면
                        {
                            candies[row, column] = candies[row2, column]; // 한칸 아래로
                            candies[row2, column] = null; // 바로 위는 비게되고

                            if (row2 - row > collapseInfo.maxDistance)
                                collapseInfo.maxDistance = row2 - row;

                            // 새로운 행,열 값 할당
                            candies[row, column].GetComponent<Candy>().Row = row;
                            candies[row, column].GetComponent<Candy>().Column = column;

                            collapseInfo.AddCandy(candies[row, column]);

                            break;
                        }
                    }
                }
            }
        }

        return collapseInfo;
    }

    // 빈공간 모두 리스트에 담아서 리턴
    public IEnumerable<CandyInfo> GetEmptyItemsOnColumn(int column)
    {
        List<CandyInfo> emptyItems = new List<CandyInfo>();

        for (int row = 0; row < GameVariables.Rows; row++)
        {
            if (candies[row, column] == null)
            {
                emptyItems.Add(new CandyInfo() { Row = row, Column = column });
            }
        }

        return emptyItems;
    }
}
