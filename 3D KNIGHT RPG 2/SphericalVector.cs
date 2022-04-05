using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SphericalVector // struct는 값형식, class는 참조형식
{
    public float Length;
    public float Zenith; // 천정
    public float Azimuth; // 방위각

    public SphericalVector(float azimuth, float zenith, float length)
    {
        Length = length;
        Zenith = zenith;
        Azimuth = azimuth;
    }

    public Vector3 Direction
    {
        get
        {
            Vector3 dir;

            float vertical_Angle = Zenith * Mathf.PI / 2f;
            dir.y = Mathf.Sin(vertical_Angle);
            float h = Mathf.Cos(vertical_Angle);

            float horizontal_Angle = Azimuth * Mathf.PI;
            dir.x = h * Mathf.Sin(horizontal_Angle);
            dir.z = h * Mathf.Cos(horizontal_Angle);

            return dir;
        }
    }

    public Vector3 Position
    {
        get
        {
            return Length * Direction;
        }
    }
}
