using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotCountText : MonoBehaviour
{
    public AnimationCurve scaleCurve;

    public CanvasGroup shotCount;

    private CanvasGroup cg;

    private Text topText, bottomText;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        topText = transform.Find("TopText").GetComponent<Text>();
        bottomText = transform.Find("BottomText").GetComponent<Text>();
        transform.localScale = Vector3.zero;
    }

    public void SetTopText(string text)
    {
        topText.text = text;
    }

    public void SetBottomText(string text)
    {
        bottomText.text = text;
    }

    public void Flash()
    {
        cg.alpha = 1; // 필요없는코드
        transform.localScale = Vector3.zero;
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i <= 60; i++)
        {
            // 애니메이션 0초대의 값부터 1초대의 값까지 localScale을 변화시킴
            transform.localScale = Vector3.one * scaleCurve.Evaluate((float)i / 50);

            // 40일때의 localScale에서 보여주고 점점 투명해지다가 i가 60일때 사라짐
            if (i >= 40) cg.alpha = (float)(60 - i) / 20;

            yield return null;
        }

        yield break;
    }
}
