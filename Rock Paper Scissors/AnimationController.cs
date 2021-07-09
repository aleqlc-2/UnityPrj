using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator playerChoiceHandlerAnimation, choiceAnimation;

    public void PlayerMadeChoice()
    {
        playerChoiceHandlerAnimation.Play("RemoveHandler");
        choiceAnimation.Play("ShowChoices");
    }

    public void ResetAnimation()
    {
        // 아무 동작도 없는 애니메이션 클립을 이용하여 리셋
        playerChoiceHandlerAnimation.Play("ShowHandler");
        choiceAnimation.Play("RemoveChoices");
    }
}
