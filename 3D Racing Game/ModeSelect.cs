using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelect : MonoBehaviour
{
    public static int RaceMode; // 0 = Race, 1 = Score, 2 = Time
    public void ScoreMode()
    {
        RaceMode = 1;
    }

    public void TimeMode()
    {
        RaceMode = 2;
    }
}
