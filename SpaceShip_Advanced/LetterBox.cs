using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBox : MonoBehaviour
{
    [SerializeField] private Camera letterBox;


    private void Start()
	{
        SetAspect((float)9f, (float)16f);
    }
	/// <summary>
	/// 레터박스를 통해 원하는 화면 비율을 세팅해주는 함수입니다.
	/// </summary>
	/// <param name="wRatio">원하는 가로 비율</param>
	/// <param name="hRatio">원하는 세로 비율</param>
	private void SetAspect(float wRatio, float hRatio)
    {
        //메인 카메라의 비율 변경을 위해 받아옵니다.
        Camera mainCam = Camera.main;

        //새로운 화면 크기 0f~1f의 값을 가집니다.
        float newScreenWidth;
        float newScreenHeight;
        //레터박스의 크기 0f~1f의 값을 가집니다.
        float letterWidth;
        float letterHeight;
        //레터박스. 레터박스는 화면을 렌더하지않는 카메라 프리팹입니다.
        Camera letterBox1 = Instantiate(letterBox);
        Camera letterBox2 = Instantiate(letterBox);

        //가로가 더 긴 비율을 원하는 경우. 상하로 레터박스가 생깁니다.
        if (wRatio > hRatio)
        {
            //새로운 화면의 가로 크기는 화면의 최대치
            newScreenWidth = 1f;
            //세로 크기는 가로를 기준으로 비율을 맞춰줍니다(newScreenWidth : newScreenHeight = wRatio : hRatio)
            newScreenHeight = newScreenWidth / wRatio * hRatio;

            //레터박스의 가로 크기는 새로운 화면의 크기와 같습니다.
            letterWidth = newScreenWidth;
            //세로 크기는 원래 화면 크기(1f)에서 새로운 화면 크기를 뺀 크기입니다.
            //위, 아래 두곳에서 생기므로 2로 나누어줍니다.
            letterHeight = (1f - newScreenHeight) * 0.5f;

            //camera.rect는 왼쪽 아래부분이 0f,0f입니다.
            //새로운 크기의 화면을 할당해줍니다. 화면의 시작점x는 0, y는 아래 레터박스의 위부터입니다.
            mainCam.rect = new Rect(0f, letterHeight, newScreenWidth, newScreenHeight);

            letterBox1.rect = new Rect(0f, 0f, letterWidth, letterHeight);//아래 레터박스
            letterBox2.rect = new Rect(0f, 1f - letterHeight, letterWidth, letterHeight);//위 레터박스
        }
        //세로가 더 긴 비율을 원하는 경우. 좌우로 레터박스가 생깁니다. 나머지는 비슷합니다.
        else
        {
            newScreenHeight = 1f;
            newScreenWidth = newScreenHeight / hRatio * wRatio;

            letterHeight = newScreenHeight;
            letterWidth = (1f - newScreenWidth) * 0.5f;


            mainCam.rect = new Rect(letterWidth, 0f, newScreenWidth, newScreenHeight);

            letterBox1.rect = new Rect(0f, 0f, letterWidth, letterHeight);//왼쪽 레터박스
            letterBox2.rect = new Rect(1f - letterWidth, 0f, letterWidth, letterHeight);//오른쪽 레터박스
        }
        //레터박스를 예쁘게 카메라의 자식으로 설정해줍니다
        letterBox1.transform.parent = mainCam.transform;
        letterBox2.transform.parent = mainCam.transform;
    }
}
