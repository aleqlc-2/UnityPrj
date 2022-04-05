using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 함수들은 Animation탭에서 이벤트만들어 메서드를 등록
public class PlayerAttackFXController : MonoBehaviour
{
    public GameObject normalAttack;
    public GameObject specialAttack_Prefab_1, specialAttack_Prefab_2, specialAttack_Prefab_3;

    public Transform normalAttack_Position;
    public Transform specialAttack_Position_1, specialAttack_Position_2, specialAttack_Position_3;
    //public Transform specialAttack_Position_2_1, specialAttack_Position_2_2;

    public bool is_Lei_Zhengzi;
    public bool is_BadeerAngel;
    public bool is_Lian_You;
    public bool is_Evil_King;
    public bool is_Dark_Sorcerer;

    private void ActivateNormalAttack()
    {
        normalAttack.SetActive(true);
    }

    private void DeactivateNormalAttack()
    {
        normalAttack.SetActive(false);
    }

    private void Spawn_SpecialAttackEffect_1()
    {
        if (is_BadeerAngel || is_Lei_Zhengzi)
            Instantiate(specialAttack_Prefab_1, specialAttack_Position_1.position, Quaternion.identity);

        if (is_Lian_You || is_Evil_King || is_Dark_Sorcerer)
            Instantiate(specialAttack_Prefab_1, specialAttack_Position_1.position, transform.rotation);
    }

    private void Spawn_SpecialAttackEffect_2()
    {
        if (is_Lei_Zhengzi || is_Evil_King)
            Instantiate(specialAttack_Prefab_2, specialAttack_Position_2.position, transform.rotation);

        if (is_BadeerAngel)
        {
            GameObject special2 = Instantiate(specialAttack_Prefab_2);
            special2.transform.position = specialAttack_Position_2.position;
            special2.transform.SetParent(specialAttack_Position_2);
        }

        if (is_Lian_You)
            Instantiate(specialAttack_Prefab_2, specialAttack_Position_2.position, Quaternion.identity);

        if (is_Dark_Sorcerer)
        {
            Instantiate(specialAttack_Prefab_2, specialAttack_Position_1.position, transform.rotation);
            Instantiate(specialAttack_Prefab_2, specialAttack_Position_2.position, transform.rotation);
            Instantiate(specialAttack_Prefab_2, specialAttack_Position_3.position, transform.rotation);
        }
    }

    private void Spawn_SpecialAttackEffect_3()
    {
        if (is_Lei_Zhengzi || is_BadeerAngel || is_Lian_You || is_Evil_King)
            Instantiate(specialAttack_Prefab_3, specialAttack_Position_3.position, transform.rotation);

        if (is_Dark_Sorcerer)
        {
            Instantiate(specialAttack_Prefab_3, specialAttack_Position_1.position, transform.rotation);
            Instantiate(specialAttack_Prefab_3, specialAttack_Position_2.position, transform.rotation);
            Instantiate(specialAttack_Prefab_3, specialAttack_Position_3.position, transform.rotation);
        }
    }

    private void Spawn_NormalAttackEffect()
    {
        Instantiate(normalAttack, normalAttack_Position.position, transform.rotation);
    }
}
