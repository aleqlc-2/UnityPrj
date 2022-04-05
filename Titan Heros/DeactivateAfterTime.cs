using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{
    public float timer = 2f;

    void Start()
    {
        Invoke("DeactivateGameObject", timer);
    }

    private void DeactivateGameObject()
    {
        gameObject.SetActive(false);
    }
}
