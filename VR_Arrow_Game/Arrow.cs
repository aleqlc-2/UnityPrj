using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public static Arrow instance;
    
    public bool isFired = false;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    void Update()
    {
        if (isFired)
        {
            // 날아가는 포물선대로 화살이 바라보게끔, velocity는 리지드바디 포지션 변화율을 나타냄.
            transform.LookAt(transform.position + transform.GetComponent<Rigidbody>().velocity);
        }
    }

    public void Fired()
    {
        isFired = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bow")
            ArrowManager.Instance.AttachBowToArrow();
    }

    // void OnTriggerStay(Collider other)
    // {
    //     if(other.tag == "Bow")
    //         ArrowManager.Instance.AttachBowToArrow();
    // }
}
