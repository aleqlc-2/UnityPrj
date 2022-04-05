using System;

[Flags] // using System; 두 속성을 하나의 변수에 담을 수 있음
public enum BonusType
{
    None,
    DestroyWholeRowColumn
}

public enum GameState
{
    None,
    SelectionStarted,
    Animating
}

public static class BonusTypeChecker
{
    public static bool ContainsDestroyWholeRowColumn(BonusType bt)
    {
        // & BonusType.DestroyWholeRowColumn?
        return (bt & BonusType.DestroyWholeRowColumn) == BonusType.DestroyWholeRowColumn;
    }
}
