using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Slingshot : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject slingGO;

    private GameObject currBall;

    private bool ready = true;

    public SteamVR_Action_Boolean trigger;
    private SteamVR_TrackedObject trackedController; // private
    private bool inSlingShot = false;

    private Vector3 slingShotStart;

    public float speed = 20f;

    void Start()
    {
        slingShotStart = slingGO.transform.position; // sling의 default지점 저장
    }

    void Update()
    {
        if (ready)
        {
            currBall = Instantiate(ballPrefab);
            currBall.transform.parent = slingGO.transform;
            currBall.transform.localPosition = Vector3.zero;
            ready = false;
        }
        
        currBall.transform.LookAt(slingShotStart); // 이거 안쓰고 실행해볼것. 다른방향으로 튀려나..?

        if (trackedController != null)
        {
            if (inSlingShot)
            {
                // GetStateUp은 원래부터 떼고 있는 경우가 아니라 눌렀다 떼는 경우임.
                if (trigger.GetStateUp(SteamVR_Input_Sources.Any))
                {
                    ready = true; // 새로운 ball 생성
                    inSlingShot = false; // 새로운 ball은 아직 날아가지 않도록

                    Vector3 ballPos = currBall.transform.position; // release할때 ball의 위치 저장

                    slingGO.transform.position = slingShotStart; // sling이 원래 위치로 복귀
                    currBall.transform.parent = null; // 발사될 공은 parent에서 뺌

                    Rigidbody r = currBall.GetComponent<Rigidbody>();

                    // slingShotStart - ballPos위치에 Vector3.forward라고 쓰면
                    // 당기는 각도에 관계없이 ball의 forward방향으로 날아가버림
                    // slingShotStart - ballPos는
                    // 당기는 각도에 따라 ball이 날아가도록 하기위함
                    r.velocity = (slingShotStart - ballPos) * speed; // 발사. 거리 * 스피드(Vector3)
                    r.useGravity = true;
                }
                else if (trigger.GetState(SteamVR_Input_Sources.Any))
                {
                    slingGO.transform.position = trackedController.transform.position;
                }
            }
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        var tController = other.GetComponent<SteamVR_TrackedObject>();
        // right 혹은 left hand가 닿았다면
        if (tController != null && (tController == trackedController || trackedController == null))
        {
            trackedController = tController;
            inSlingShot = true;
        }
    }

    void OnTriggerExit(Collider other) // other가 트리거에서 벗어났을 때
    {
        var tController = other.GetComponent<SteamVR_TrackedObject>();
        if (tController != null && trackedController == tController)
        {
            trackedController = null;
            inSlingShot = false;
        }
    }
}
