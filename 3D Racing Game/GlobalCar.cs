using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCar : MonoBehaviour
{
    public static int CarType; // 1 = Car01, 2 = Car02
    public GameObject TrackWindow;

    public void Car01()
    {
        CarType = 1;
        TrackWindow.SetActive(true);
    }

    public void Car02()
    {
        CarType = 2;
        TrackWindow.SetActive(true);
    }
}
