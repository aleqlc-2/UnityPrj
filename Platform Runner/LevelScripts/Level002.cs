using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level002 : MonoBehaviour
{
    public GameObject fadeIn;

    void Start()
    {
        // Debug.Log(this.gameObject.name + " 입니다."); // 스크립트가 부착된 오브젝트 찾기

        RedirectToLevel.redirectToLevel = 5;
        //RedirectToLevel.nextLevel = 6; // Level003
        StartCoroutine(FadeInOff());
    }

    IEnumerator FadeInOff()
    {
        // fadein 객체는 하이어라키창에서 활성화되어있으므로 게임 시작시 1초간 fadein되고
        yield return new WaitForSeconds(1); // 1초 후에
        fadeIn.SetActive(false); // 객체 비활성화
    }
}