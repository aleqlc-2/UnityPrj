using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public ARFaceManager faceManager;

	public Material[] faceMats;

	public Text indexText;
	private int vertNum = 0;
	private int vertCount = 468;

	void Start()
	{
		// 최초의 인덱스값을 0으로 초기화
		indexText.text = vertNum.ToString();
	}

	// 버튼눌렀을때 실행될 함수
	public void ToggleMaskImage()
	{
		// faceManager 컴포넌트에서 현재 생성된 Face 오브젝트를 모두 순회
		foreach (ARFace face in faceManager.trackables)
		{
			// 만일 얼굴을 인식하고 있는 상태라면
			if (face.trackingState == TrackingState.Tracking)
			{
				// Face 오브젝트의 활성화를 토글
				face.gameObject.SetActive(!face.gameObject.activeSelf);
			}
		}
	}

	// 매터리얼 변경 버튼 함수
	public void SwitchFaceMaterial(int num)
	{
		foreach (ARFace face in faceManager.trackables)
		{
			if (face.trackingState == TrackingState.Tracking)
			{
				MeshRenderer mr = face.GetComponent<MeshRenderer>();
				mr.material = faceMats[num];
			}
		}
	}

	public void IndexIncrease()
	{
		// vertNum의 값을 1 증가시키되, 최대인덱스를 넘지않도록한다.
		int number = Mathf.Min(++vertNum, vertCount - 1);
		indexText.text = number.ToString();
	}

	public void IndexDecrease()
	{
		// vertNum의 값을 1 감소시키되, 최소인덱스보다 작지않도록한다.
		int number = Mathf.Max(--vertNum, 0);
		indexText.text = number.ToString();
	}
}
