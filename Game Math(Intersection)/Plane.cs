using Unity.VisualScripting;
using UnityEngine;

public class Plane
{
    public CoordsInter A;
    CoordsInter B;
    CoordsInter C;
    public CoordsInter v;
    public CoordsInter u;

    public Vector3 Avector3;
    public Vector3 Bvector3;
    public Vector3 Cvector3;

    // A�� ����� B,C�� A���� ����ϴ� ���δٸ� �� ����
    public Plane(CoordsInter _A, CoordsInter _B, CoordsInter _C)
    {
        A = _A;
        B = _B;
        C = _C;
        v = B - A;
        u = C - A;
    }

    public Plane(CoordsInter _A, Vector3 V, Vector3 U)
    {
        A = _A;
        v = new CoordsInter(V.x, V.y, V.z);
        u = new CoordsInter(U.x, U.y, U.z);
    }

    public CoordsInter Lerp(float s, float t)
    {
        float xst = A.x + v.x * s + u.x * t;
        float yst = A.y + v.y * s + u.y * t;
        float zst = A.z + v.z * s + u.z * t;

        return new CoordsInter(xst, yst, zst);
    }
}
