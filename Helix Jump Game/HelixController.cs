using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelixController : MonoBehaviour
{
    private Vector2 lastTapPos; // 디폴트값은 Vector2.zero
    private Vector3 startRotation;

    public Transform topTransform;
    public Transform goalTransform;
    public GameObject helixLevelPrefab;
    public List<Stage> allStages = new List<Stage>(); // 인스펙터창에서 할당했음
    private float helixDistance;
    private List<GameObject> spawnedLevels = new List<GameObject>();

    void Awake()
    {
        startRotation = transform.localEulerAngles;
        helixDistance = topTransform.localPosition.y - (goalTransform.localPosition.y + 0.1f);
        LoadStage(0);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 curTapPos = Input.mousePosition; // 현재 클릭 중인 마우스 위치 저장

            if (lastTapPos == Vector2.zero) // 마우스 움직이면 이 구문에 들어올 수 없음
            {
                lastTapPos = curTapPos; // 클릭 후 가장 처음 마우스 위치가 lastTapPos에 저장됨
            }

            float delta = lastTapPos.x - curTapPos.x; // 움직이기 전 마우스 위치 - 움직인 후 마우스 위치
            lastTapPos = curTapPos; // 움직인 후 마우스 위치 저장

            transform.Rotate(Vector3.up * delta); // y축을 기준으로 Rotate
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastTapPos = Vector2.zero; // 마우스 떼면 초기화
        }
    }

    public void LoadStage(int stageNumber)
    {
        Stage stage = allStages[Mathf.Clamp(stageNumber, 0, allStages.Count - 1)]; // Clamp는 범위설정

        if (stage == null)
        {
            Debug.LogError("No stage " + stageNumber + " found in allStages List," +
                " Are all stages assined in the list?");
            return;
        }

        // 스테이지의 배경색 변경
        Camera.main.backgroundColor = allStages[stageNumber].stageBackgroundColor;

        // 스테이지의 볼 색깔 변경
        // FindObjectOfType가 스크립트가 부착된 오브젝트를 반환하는듯
        FindObjectOfType<BallController>().GetComponent<Renderer>().material.color =
            allStages[stageNumber].stageBallColor;

        // 원판 rotation 리셋
        transform.localEulerAngles = startRotation;

        // 존재하는 이전 원판 Destroy
        foreach (GameObject go in spawnedLevels)
            Destroy(go);

        // 기둥 위에서부터 내려가면서 원판 생성
        float levelDistance = helixDistance / stage.levels.Count; // stage.levels는 인스펙터창에서 할당했음
        float spawnPosY = topTransform.localPosition.y;

        for (int i = 0; i < stage.levels.Count; i++)
        {
            spawnPosY -= levelDistance;
            GameObject level = Instantiate(helixLevelPrefab, transform);
            level.transform.localPosition = new Vector3(0, spawnPosY, 0); // 생성위치 설정
            spawnedLevels.Add(level);

            // stage 0 의 partCount를 9로 설정하였으므로 12-9=3
            int partsToDisable = 12 - stage.levels[i].partCount;
            List<GameObject> disableParts = new List<GameObject>();

            // 1~3개가 비활성화됨. 비활성화 된 것을 중복으로 선택가능하며 다른 개체로 인식하는듯.
            // 밑에 if문 쓸모없는 코드
            // if문 몸체코드 항상 실행되며 다른개체라서 disableParts.Count 항상 +1 됨
            while (disableParts.Count < partsToDisable)
            {
                // 원판의 여러개의 조각중 하나를 랜덤으로 골라 randomPart에 담는다
                GameObject randomPart = level.transform.GetChild(
                    Random.Range(0, level.transform.childCount)).gameObject;
                //if (!disableParts.Contains(randomPart)) // disable 된 조각이 아니라면
                //{
                    randomPart.SetActive(false); // 비활성화하고
                    disableParts.Add(randomPart); // disable시킨 조각은 disableParts에 저장
                //}
            }

            List<GameObject> leftParts = new List<GameObject>();
            foreach (Transform t in level.transform)
            {
                // 모든 조각의 칼라를 바꾸고
                t.GetComponent<Renderer>().material.color = allStages[stageNumber].stageLevelPartColor;
                if (t.gameObject.activeInHierarchy) // 비활성화된 조각은 List에 안들어가도록
                    leftParts.Add(t.gameObject);
            }

            List<GameObject> deathParts = new List<GameObject>();
            // 위의 while과 마찬가지로 if문 무쓸모
            while (deathParts.Count < stage.levels[i].deathPartCount) 
            {
                // 랜덤으로 게임오버조각 생성
                GameObject randomPart = leftParts[(Random.Range(0, leftParts.Count))];
                //if (!deathParts.Contains(randomPart))
                //{
                    randomPart.AddComponent<DeathPart>();
                    deathParts.Add(randomPart);
                //}
            }
        }
    }
}
