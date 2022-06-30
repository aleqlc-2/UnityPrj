using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MultipleImageTracker : MonoBehaviour
{
    private ARTrackedImageManager imageManager;

    void Start()
    {
        imageManager = GetComponent<ARTrackedImageManager>();

        // 이미지 인식 델리게이트에 실행될 함수를 연결
        imageManager.trackedImagesChanged += OnTrackedImage;
    }

    public void OnTrackedImage(ARTrackedImagesChangedEventArgs args)
	{
        // 새로 인식한 이미지들을 모두 순회한다.
		foreach (ARTrackedImage trackedImage in args.added)
		{
            // 이미지 라이브러리에서 인식한 이미지의 이름을 가져온다
            string imageName = trackedImage.referenceImage.name;

            // Resources 폴더에서 인식한 이미지의 이름과 동일한 이름의 프리펩을 찾는다.
            GameObject imagePrefab = Resources.Load<GameObject>(imageName);

            if (imagePrefab != null)
			{
                if (trackedImage.transform.childCount < 1) // 이미 생성된 프리펩이 없다면
				{
                    // 이미지 위치에 프리펩을 생성하고 이미지의 자식오브젝트로 등록
                    GameObject go = Instantiate(imagePrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                    go.transform.SetParent(trackedImage.transform);
                }
			}
		}

		foreach (ARTrackedImage trackedImage in args.updated)
		{
            // 이미 생성된 프리펩이 있다면
            if (trackedImage.transform.childCount > 0)
			{
                // 인식된 이미지의 위치가 변경되면 자식개체인 프리펩의 위치도 변경되도록
                trackedImage.transform.GetChild(0).position = trackedImage.transform.position;
                trackedImage.transform.GetChild(0).rotation = trackedImage.transform.rotation;
			}
		}
	}
}
