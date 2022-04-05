using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationMainMenu : MonoBehaviour
{
    private Animator myAnim;

    void Awake()
    {
        myAnim = GetComponent<Animator>();
    }

    // 인스펙터창에서 미리 applyRootMotion 비활성화하고 SlideIn 애니메이션이 끝나는시점에 이벤트등록해서 true해줘야 동작이 제대로 됨.
    // 또한, Animator에서 디폴트상테는 Empty임
    // private여도 이벤트 등록되네.
    private void TurnOnRootMotion()
    {
        myAnim.applyRootMotion = true;
    }

    // 캐릭터 SlideIn도중에 등록
    // Ground의 Animator에 applyRootMotion 미리 비활성화해놔야함. 체크해놓으면 동작 이상함
    private void AppearGround()
    {
        MainMenuAnimationsController.instance.GroundSlideIn();
    }

    private void ThunderEffect()
    {
        MainMenuAnimationsController.instance.ActivateThunderFX();
    }


    private void HeroAppearSound()
    {
        MainMenuAnimationsController.instance.PlayHeroAppearSound();
    }
}
