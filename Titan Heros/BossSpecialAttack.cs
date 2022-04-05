using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpecialAttack : MonoBehaviour
{
    public GameObject specialAttack_Prefab;

    public Transform specialAttack_Point;

    private void SpawnSpecialAttack()
    {
        Instantiate(specialAttack_Prefab, specialAttack_Point.position, Quaternion.identity);
    }
}
