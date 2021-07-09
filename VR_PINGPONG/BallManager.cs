using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public GameObject BallPrefab;

    [HideInInspector]
    public GameObject CurrentBall;

    public static BallManager Instance;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    void Destroy()
    {
        if(Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (CurrentBall == null)
            CurrentBall = Instantiate(BallPrefab, transform.position, transform.rotation) as GameObject;
    }
}
