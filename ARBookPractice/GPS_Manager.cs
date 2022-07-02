using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GPS_Manager : MonoBehaviour
{
    public static GPS_Manager instance;

    public Text latitude_text;
    public Text longitude_text;

    public float latitude = 0;
    public float longitude = 0;

    public float maxWaitTime = 10.0f; // 최대 응답 대기시간
    private float waitTime = 0f; // 현재 경과된 대기시간

    public float resendTime = 1.0f;
    public bool receiveGPS = false;

	void Awake()
	{
        if (instance == null)
            instance = this;
	}

	void Start()
    {
        StartCoroutine(GPS_On());
    }
    public IEnumerator GPS_On()
	{
        // GPS사용허가를 받지못했다면 권한허가 팝업을 띄운다
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
		{
            Permission.RequestUserPermission(Permission.FineLocation);
            
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
			{
                yield return null;
			}
		}

        // 모바일의 GPS 장치가 꺼져있으면 위치정보를 수신할 수 없다고 표시하고 코루틴종료
        if (!Input.location.isEnabledByUser)
		{
            latitude_text.text = "GPS Off";
            longitude_text.text = "GPS Off";
            yield break;
		}

        // 위치데이터 요청 -> 수신대기
        Input.location.Start();

        // GPS 수신상태가 초기상태에서 maxWaitTime동안 대기한다.
        while (Input.location.status == LocationServiceStatus.Initializing && waitTime < maxWaitTime)
		{
            yield return new WaitForSeconds(1.0f);
            waitTime++;
		}

        // 수신실패 시 실패출력
        if (Input.location.status == LocationServiceStatus.Failed)
		{
            latitude_text.text = "위치 정보 수신 실패";
            longitude_text.text = "위치 정보 수신 실패";
		}

        // 응답대기시간 초과시
        if (waitTime > maxWaitTime)
		{
            latitude_text.text = "응답 대기 시간 초과";
            longitude_text.text = "응답 대기 시간 초과";
        }

        // 수신된 GPS 데이터를 화면에 출력
        LocationInfo li = Input.location.lastData;
        latitude = li.latitude;
        longitude = li.longitude;
        latitude_text.text = "위도 : " + latitude.ToString();
        longitude_text.text = "경도 : " + longitude.ToString();

        // 첫수신시도 이후 while로 계속 돌면서 resendTime마다 변동사항있는지 체크하여 변경사항 적용
        receiveGPS = true;
        while (receiveGPS)
		{
            yield return new WaitForSeconds(resendTime);
            li = Input.location.lastData;
            latitude = li.latitude;
            longitude = li.longitude;
            latitude_text.text = "위도 : " + latitude.ToString();
            longitude_text.text = "경도 : " + longitude.ToString();
        }
	}
}
