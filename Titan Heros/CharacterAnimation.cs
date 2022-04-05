using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Walk(bool walk)
    {
        anim.SetBool(AnimationTags.WALK_PARAMETER, walk);
    }

    public void Run(bool run)
    {
        anim.SetBool(AnimationTags.RUN_PARAMETER, run);
    }

    public void NormalAttack_1()
    {
        anim.SetTrigger(AnimationTags.NORMAL_ATTACK_1_TRIGGER);
    }

    public void NormalAttack_2()
    {
        anim.SetTrigger(AnimationTags.NORMAL_ATTACK_2_TRIGGER);
    }

    public void SpecialAttack_1()
    {
        anim.SetTrigger(AnimationTags.SPECIAL_ATTACK_1_TRIGGER);
    }

    public void SpecialAttack_2()
    {
        anim.SetTrigger(AnimationTags.SPECIAL_ATTACK_2_TRIGGER);
    }

    public void SpecialAttack_3()
    {
        anim.SetTrigger(AnimationTags.SPECIAL_ATTACK_3_TRIGGER);
    }

    public void Hit()
    {
        anim.SetTrigger(AnimationTags.HIT_TRIGGER);
    }

    public void Dead()
    {
        anim.SetTrigger(AnimationTags.DEAD_TRIGGER);
    }

    public void StopAnimation()
    {
        anim.StopPlayback(); // 애니메이션 중지
    }

    private void BackToIdle()
    {
        anim.Play(AnimationTags.IDLE_ANIMATION); // 상태이름을 인자로
    }
}
