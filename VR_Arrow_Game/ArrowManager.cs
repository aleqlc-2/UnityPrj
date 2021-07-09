using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

// righthand에 부착된 스크립트
public class ArrowManager : MonoBehaviour
{
    public static ArrowManager Instance;
    public SteamVR_Behaviour_Pose trackedObj; // SteamVR -> Extras -> TestThrow.cs
    private GameObject currentArrow;
    public GameObject stringAttachPoint;
    private Vector3 initialStringAttachPoint;
    //public GameObject arrowPrefab;
    public GameObject arrowStartPoint;
    
    public GameObject stringStartPoint;
    private bool isAttached = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Start()
    {
        initialStringAttachPoint = stringAttachPoint.transform.localPosition;
    }

    void Update()
    {
        //AttachArrow();
        PullString();
    }

    //private void AttachArrow()
    //{
        //if (currentArrow == null)
        //{
            //currentArrow = Instantiate(arrowPrefab);
            // currentArrow.transform.parent = trackedObj.transform;
            // currentArrow.transform.localPosition = new Vector3(0f, 0f, 0.25f);
        //}
    //}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Arrow")
        {
            other.tag = "Ready";
            other.transform.position = trackedObj.transform.position; // 위치주고
            other.transform.parent = trackedObj.transform; // child화하여 화살이 손의 위치 따라가도록
            currentArrow = other.gameObject;
        }
    }

    public void AttachBowToArrow()
    {
        currentArrow.transform.parent = stringAttachPoint.transform; // child화
        currentArrow.transform.localPosition = arrowStartPoint.transform.localPosition; // 위치주고
        currentArrow.transform.rotation = arrowStartPoint.transform.rotation; // 회전주고
        currentArrow.transform.Rotate(90f, 0, 0); // 하드코딩으로 위치맞춤
        
        isAttached = true;
    }

    private void PullString()
    {
        if (isAttached)
        {
            float dist = (stringStartPoint.transform.position - trackedObj.transform.position).magnitude;
            
            // 당기는대로 이동
            stringAttachPoint.transform.localPosition
             = stringStartPoint.transform.localPosition + new Vector3(0f, -5 * dist, 0f);

            if (dist >= 0.7f)
            {
                stringAttachPoint.transform.localPosition = initialStringAttachPoint;
                StartCoroutine(Fire());
            }
        }
    }

    IEnumerator Fire()
    {
        Arrow.instance.Fired(); // currentArrow.GetComponent<Arrow>().Fired(); // 이렇게 호출하면 싱글턴 안써도됨.
        currentArrow.transform.parent = null;

        // 당긴만큼 멀리나감
        float dist = (stringStartPoint.transform.position - trackedObj.transform.position).magnitude;
        Rigidbody r = currentArrow.GetComponent<Rigidbody>();

        // currentArrow.transform.forward가 방향, 25f * dist가 스피드인듯
        r.velocity = currentArrow.transform.forward * 25f * dist;
        r.useGravity = true; // 중력의 영향으로 포물선으로 날아가도록

        currentArrow.GetComponent<Collider>().isTrigger = false; // 화살이 bow외의 다른 물체와 부딪힐 수 있도록

        currentArrow = null; // 변수 비워줌
        isAttached = false;

        yield return new WaitForSeconds(3f);
        Destroy(GameObject.FindWithTag("Ready"));
    }
}
