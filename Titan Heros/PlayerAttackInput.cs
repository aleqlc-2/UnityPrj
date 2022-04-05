using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackInput : MonoBehaviour
{
    private CharacterAnimation playerAnimation;

    public bool is_Lian_You;

    void Start()
    {
        playerAnimation = GetComponent<CharacterAnimation>();
    }

    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // LianYou만 기본공격패턴이 2가지
            if (is_Lian_You)
            {
                if (Random.Range(0, 2) > 0) playerAnimation.NormalAttack_1();
                else playerAnimation.NormalAttack_2();
            }
            else
            {
                playerAnimation.NormalAttack_1();
            }
        }

        if (Input.GetKeyDown(KeyCode.J)) playerAnimation.SpecialAttack_1();
        if (Input.GetKeyDown(KeyCode.K)) playerAnimation.SpecialAttack_2();
        if (Input.GetKeyDown(KeyCode.L)) playerAnimation.SpecialAttack_3();
    }
}
