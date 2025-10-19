using UnityEngine;

public class HolisticMath
{
    // normalized, 정규화
    static public Coords GetNormal(Coords vector)
    {
        float length = Distance(new Coords(0, 0, 0), vector);
		vector.x /= length;
        vector.y /= length;
        vector.z /= length;

        return vector;
    }

    // magnitude 구하기
    static public float Distance(Coords point1, Coords point2)
    {
		float diffSquared = Square(point1.x - point2.x) + Square(point1.y - point2.y) + Square(point1.z - point2.z); // 두 점 차이의 제곱의 합
        float squareRoot = Mathf.Sqrt(diffSquared); // 루트씌움
        return squareRoot; // 빗변의 길이, 두 점사이의 거리
	}

    // 제곱
    static public float Square(float value)
    {
        return value * value;
    }

    // 두 벡터의 내적. 90도보다 작으면 양수, 90도면 0, 90도 이상이면 음수
    static public float Dot(Coords vector1, Coords vector2)
    {
        return (vector1.x * vector2.x + vector2.y * vector2.y + vector1.z * vector2.z);
    }

    // 내적을 이용해 두 벡터사이의 각도구하기, 각도 = 코사인역함수 * (dot / 두벡터 거리의 곱)
    static public float Angle(Coords vector1, Coords vector2)
    {
        float dotDivide = Dot(vector1, vector2) / (Distance(new Coords(0, 0, 0), vector1) * Distance(new Coords(0, 0, 0), vector2));
        dotDivide = Mathf.Clamp(dotDivide, -1, 1);
		return Mathf.Acos(dotDivide); // radians값. degrees로 바꾸려면 180/Mathf.PI 곱하면 됨
    }

    // 벡터가 회전함에 따른 변화각을 이용해 회전완료된 벡터의 x,y값 구하기, 라디안으로
    // 매개변수는 회전 전 벡터(계산의 기준축)의 x,y , angle은 회전 후의 각도
    static public Coords Rotate(Coords vector, float angle, bool clockwise)
    {
		//Debug.Log(angle * 180/Mathf.PI); // 180/Mathf.PI = Mathf.Rad2Deg
		if (clockwise) angle = 2 * Mathf.PI - angle; // 2 * Mathf.PI는 360도의 라디안값
        //Debug.Log(angle * 180/Mathf.PI);
		float xVal = vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle);
        float yVal = vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle);
        return new Coords(xVal, yVal, 0);
    }

    static public Coords Translate(Coords position, Coords facing, Coords translation)
    {
        if (HolisticMath.Distance(new Coords(0, 0, 0), translation) <= 0) return position;

		float angle = HolisticMath.Angle(translation, facing);
		float worldAngle = HolisticMath.Angle(translation, new Coords(0, 1, 0));
		bool clockwise = false;
        if (HolisticMath.Cross(translation, facing).z < 0) clockwise = true;

		translation = HolisticMath.Rotate(translation, angle + worldAngle, clockwise);
		
		float xVal = position.x + translation.x;
        float yVal = position.y + translation.y;
        float zVal = position.z + translation.z;
		return new Coords(xVal, yVal, zVal);
    }

    static public Coords Cross(Coords vector1, Coords vector2)
    {
		float xMult = vector1.y * vector2.z - vector1.z * vector2.y;
        float yMult = vector1.z * vector2.x - vector1.x * vector2.z;
        float zMult = vector1.x * vector2.y - vector1.y * vector2.x;
        Coords crossProd = new Coords(xMult, yMult, zMult);
        return crossProd;
    }
}
