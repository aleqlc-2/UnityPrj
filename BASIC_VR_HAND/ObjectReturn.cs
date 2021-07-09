using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReturn : MonoBehaviour
{
    public float waitTime = 2.0f; // 리턴딜레이
    
    public Transform objTransform = null; // 원래 Transform값

    private Rigidbody objRigidbody = null; // velocity를 초기화 하기위한 변수

    private Vector3 myOriginPosition = Vector3.zero; // 초기 위치값 저장할 변수
    
    private Quaternion myOriginRotation = Quaternion.identity; // 초기 회전값 저장할 변수

    private Coroutine myCoroutine = null; // 코루틴을 저장하는 변수

    void Start()
    {
        objRigidbody = GetComponent<Rigidbody>();
        objTransform = GetComponent<Transform>(); // 여기서 할당했으므로 인스펙터창에서 넣을 필요 없음

        myOriginPosition = objTransform.position; // 초기 위치값 저장
        myOriginRotation = objTransform.rotation; // 초기 회전값 저장
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        // Origin과 Interactable 이외의 물체와 충돌이 일어났을 때에만 리턴함수 호출이므로
        // Origin 또는 Interactable끼리 충돌했을때는 리턴이 일어나지 않음
        if (!collision.gameObject.CompareTag("Origin") && !collision.gameObject.CompareTag("Interactable"))
        {
            if (myCoroutine == null) // 코루틴이 계속 호출되지 않도록
            {
                // 오브젝트를 리턴하는 함수를 호출
                myCoroutine = StartCoroutine(ReturnObject());
            }
            
        }
    }

    // 오브젝트를 리턴하는 코루틴 함수
    IEnumerator ReturnObject()
    {
        yield return new WaitForSeconds(waitTime);
        
        // 초기화
        objRigidbody.velocity = Vector3.zero; 
        objRigidbody.angularVelocity = Vector3.zero;
        objTransform.position = myOriginPosition;
        objTransform.rotation = myOriginRotation;

        myCoroutine = null; // 코루틴 실행 후 코루틴 변수 초기화
        
        yield return null; // 이 코드는 생략가능
    }
}
