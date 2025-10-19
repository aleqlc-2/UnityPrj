using Unity.VisualScripting;
using UnityEngine;

public class Line
{
    public CoordsInter A;
    public CoordsInter B;
    public CoordsInter v;

    public enum LINETYPE { LINE, SEGMENT, RAY };
    LINETYPE type;

    public Line(CoordsInter _A, CoordsInter _B, LINETYPE _type)
    {
        A = _A;
        B = _B;
        type = _type;
		v = new CoordsInter(B.x - A.x, B.y - A.y, B.z - A.z);
    }

    public Line(CoordsInter _A, CoordsInter _V)
    {
        A = _A;
        v = _V;
        B = _A + v;
        type = LINETYPE.SEGMENT;
    }

    // r = a - 2(a.n)n
    public CoordsInter Reflect(CoordsInter normal)
    {
        CoordsInter norm = normal.GetNormal(); // wall
        CoordsInter vnorm = v.GetNormal(); // trajectory(ball의 path)

		float d = HolisticMathInter.Dot(norm, vnorm);
        if (d == 0) return v;
        float vn2 = d * 2;
        CoordsInter r = vnorm - norm * vn2;
        return r;
    }

    // t = n.(A-B)/n.w  평면과 직선이 교차할때 t값 구하기
    public float IntersectsAt(Plane p)
    {
        CoordsInter normal = HolisticMathInter.Cross(p.u, p.v);
        if (HolisticMathInter.Dot(normal, v) == 0) return float.NaN;
        float t = HolisticMathInter.Dot(normal, p.A - A) / HolisticMathInter.Dot(normal, v);
        return t;
    }

    // t = u벡터의직교*c / u벡터의직교*v, 두 벡터의 교차점을 구하기위해 직선상의 이동비율 t를 구하는과정
    public float IntersectsAt(Line l)
    {
        if (HolisticMathInter.Dot(CoordsInter.Perp(l.v), v) == 0) return float.NaN;

        CoordsInter c = l.A - this.A; // l.A는 매개변수의 A, this.A는 이 함수를 호출한 L의 A
        float t = HolisticMathInter.Dot(CoordsInter.Perp(l.v), c) / HolisticMathInter.Dot(CoordsInter.Perp(l.v), v);
        if ((t < 0 || t > 1) && type == LINETYPE.SEGMENT) return float.NaN;
        return t;
    }

    public void Draw(float width, Color col)
    {
        CoordsInter.DrawLine(A, B, width, col);
    }

    // A에서 B방향으로 가는데 t가 0.25면 25%의 위치, 0.5면 50%위치 time.time이면 계속이동
    public CoordsInter Lerp(float t)
    {
        if (type == LINETYPE.SEGMENT)
            t = Mathf.Clamp(t, 0, 1);
        else if (type == LINETYPE.RAY && t < 0)
            t = 0;

        float xt = A.x + v.x * t;
        float yt = A.y + v.y * t;
        float zt = A.z + v.z * t;

        return new CoordsInter(xt, yt, zt);
    }
}
