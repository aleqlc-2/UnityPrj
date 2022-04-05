using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오브젝트에 부착될 스크립트는 아니지만 transform.position 쓰므로 MonoBehaviour 상속해야함
public class Orbit : MonoBehaviour
{
    public SphericalVector spherical_Vector_Data = new SphericalVector(0, 0, 1);

    protected virtual void Update()
    {
        transform.position = spherical_Vector_Data.Position;
    }
}
