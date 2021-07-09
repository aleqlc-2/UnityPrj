using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarChoice : MonoBehaviour
{
    public GameObject Car01Body;
    public GameObject Car02Body;
    public int CarImport;

    void Start()
    {
        CarImport = GlobalCar.CarType;
        if (CarImport == 1)
            Car01Body.SetActive(true);
        else if (CarImport == 2)
            Car02Body.SetActive(true);
    }
}
