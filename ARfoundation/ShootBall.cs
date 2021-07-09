using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 카메라에서 공 발사됨, 발사루틴 미구현?
public class ShootBall : MonoBehaviour
{
    public GameObject _theBall; // Ball 프리펩에 리지드바디 넣어야함, 단 raycast예제에서는 리지드바디 없어야함
    public Transform _camObj;
    public Transform _shootPoint;

    private float speed = 100f;

    public void ShootBallObj()
    {
        GameObject tObj = Instantiate(_theBall);
        tObj.transform.position = _shootPoint.transform.position;

        // _shootPoint가 카메라에서 z축으로 0.01이동시킨 위치이므로
        // 카메라가 바라보는 방향
        Vector3 tVec = (_shootPoint.transform.position - _camObj.transform.position).normalized;

        Rigidbody tR = tObj.GetComponent<Rigidbody>();
        tR.AddForce(tVec * speed);
    }
}
