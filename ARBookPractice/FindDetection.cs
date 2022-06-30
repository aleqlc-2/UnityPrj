using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARCore;
using Unity.Collections;
using UnityEngine.UI;

public class FindDetection : MonoBehaviour
{
    public ARFaceManager afm;
    public GameObject smallCube;

    List<GameObject> faceCubes = new List<GameObject>();

    ARCoreFaceSubsystem subSys;

    NativeArray<ARCoreFaceRegionData> regionData; // using Unity.Collections;

    public Text vertexIndex;

    void Start()
    {
		for (int i = 0; i < 3; i++)
		{
            GameObject go = Instantiate(smallCube);
            faceCubes.Add(go);
            go.SetActive(false);
		}

        // AR Face Manager가 얼굴을 인식할 때 실행할 함수를 연결
        // 메시방식 쓰려면 OnDetectFaceAll를 연결
        afm.facesChanged += OnDetectThreePoints;

        // AR Foundation의 XRFaceSubsystem 클래스 변수를 AR Core의 ARCoreFaceSubsystem 클래스 변수로 캐스팅
        subSys = (ARCoreFaceSubsystem)afm.subsystem;
    }

    // facesChanged 델리게이트(Action)에 연결할 함수
    private void OnDetectThreePoints(ARFacesChangedEventArgs args)
	{
        // 얼굴인식정보가 갱신된것이 있다면(얼굴을 추적중이라면)
        if (args.updated.Count > 0)
		{
            // 인식된 얼굴에서 특정 위치를 가져온다
            subSys.GetRegionPoses(args.updated[0].trackableId, Allocator.Persistent, ref regionData);

            // 인식된 특정 부위의 위치로 큐브를 이동
			for (int i = 0; i < regionData.Length; i++)
			{
                faceCubes[i].transform.position = regionData[i].pose.position;
                faceCubes[i].transform.rotation = regionData[i].pose.rotation;
                faceCubes[i].SetActive(true);
			}
		}

        // 얼굴인식정보를 잃었다면(얼굴추적이 안되고 있다면)
        if (args.removed.Count > 0)
		{
            // 큐브 비활성화
			for (int i = 0; i < regionData.Length; i++)
			{
                faceCubes[i].SetActive(false);
			}
		}
	}

    // 얼굴 메시 데이터를 이용한 방식
    private void OnDetectFaceAll(ARFacesChangedEventArgs args)
	{
        if (args.updated.Count > 0)
		{
            // +-버튼눌러 텍스트 UI에 적힌 문자열 데이터를 정수형 데이터로 변환한다.
            int num = int.Parse(vertexIndex.text);

            // 얼굴 정점 배열에서 지정한 인덱스에 해당하는 좌표를 가져온다
            Vector3 vertPosition = args.updated[0].vertices[num];

            // 정점좌표를 월드좌표로 변환하여 vertPosition값 갱신
            vertPosition = args.updated[0].transform.TransformPoint(vertPosition);

            // 준비된 큐브 하나를 활성화하고 정점 위치에 가져다 놓는다.
            faceCubes[0].SetActive(true);
            faceCubes[0].transform.position = vertPosition;
		}
        else if (args.removed.Count > 0)
		{
            faceCubes[0].SetActive(false);
		}
	}
}
