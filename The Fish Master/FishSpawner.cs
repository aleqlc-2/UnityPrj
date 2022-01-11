using System;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private Fish fishPrefab;

    [SerializeField] private Fish.FishType[] fishTypes;

    void Awake()
    {
        for (int i = 0; i < fishTypes.Length; i++)
        {
            int num = 0;
            while (num < fishTypes[i].fishCount)
            {
                Fish fish = Instantiate<Fish>(fishPrefab);
                fish.Type = fishTypes[i];
                fish.ResetFish();
                num++;
            }
        }
    }
}
