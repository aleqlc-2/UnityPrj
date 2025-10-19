using UnityEngine;

public class HolisticMathInter
{
	// normalized, ����ȭ
	static public CoordsInter GetNormal(CoordsInter vector)
	{
		float length = Distance(new CoordsInter(0, 0, 0), vector);
		vector.x /= length;
		vector.y /= length;
		vector.z /= length;

		return vector;
	}

	// magnitude ���ϱ�
	static public float Distance(CoordsInter point1, CoordsInter point2)
	{
		float diffSquared = Square(point1.x - point2.x) + Square(point1.y - point2.y) + Square(point1.z - point2.z); // �� �� ������ ������ ��
		float squareRoot = Mathf.Sqrt(diffSquared); // ��Ʈ����
		return squareRoot; // ������ ����, �� �������� �Ÿ�
	}

	// ����
	static public float Square(float value)
	{
		return value * value;
	}

	// �� ������ ����. 90������ ������ ���, 90���� 0, 90�� �̻��̸� ����
	static public float Dot(CoordsInter vector1, CoordsInter vector2)
	{
		return (vector1.x * vector2.x + vector2.y * vector2.y + vector1.z * vector2.z);
	}

	// ������ �̿��� �� ���ͻ����� �������ϱ�, ���� = �ڻ��ο��Լ� * (dot / �κ��� �Ÿ��� ��)
	static public float Angle(CoordsInter vector1, CoordsInter vector2)
	{
		float dotDivide = Dot(vector1, vector2) / (Distance(new CoordsInter(0, 0, 0), vector1) * Distance(new CoordsInter(0, 0, 0), vector2));
		dotDivide = Mathf.Clamp(dotDivide, -1, 1);
		return Mathf.Acos(dotDivide); // radians��. degrees�� �ٲٷ��� 180/Mathf.PI ���ϸ� ��
	}

	// ���Ͱ� ȸ���Կ� ���� ��ȭ���� �̿��� ȸ���Ϸ�� ������ x,y�� ���ϱ�, ��������
	// �Ű������� ȸ�� �� ����(����� ������)�� x,y , angle�� ȸ�� ���� ����
	static public CoordsInter Rotate(CoordsInter vector, float angle, bool clockwise)
	{
		//Debug.Log(angle * 180/Mathf.PI); // 180/Mathf.PI = Mathf.Rad2Deg
		if (clockwise) angle = 2 * Mathf.PI - angle; // 2 * Mathf.PI�� 360���� ���Ȱ�
													 //Debug.Log(angle * 180/Mathf.PI);
		float xVal = vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle);
		float yVal = vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle);
		return new CoordsInter(xVal, yVal, 0);
	}

	static public CoordsInter Translate(CoordsInter position, CoordsInter facing, CoordsInter translation)
	{
		if (HolisticMathInter.Distance(new CoordsInter(0, 0, 0), translation) <= 0) return position;

		float angle = HolisticMathInter.Angle(translation, facing);
		float worldAngle = HolisticMathInter.Angle(translation, new CoordsInter(0, 1, 0));
		bool clockwise = false;
		if (HolisticMathInter.Cross(translation, facing).z < 0) clockwise = true;

		translation = HolisticMathInter.Rotate(translation, angle + worldAngle, clockwise);

		float xVal = position.x + translation.x;
		float yVal = position.y + translation.y;
		float zVal = position.z + translation.z;
		return new CoordsInter(xVal, yVal, zVal);
	}

	static public CoordsInter Cross(CoordsInter vector1, CoordsInter vector2)
	{
		float xMult = vector1.y * vector2.z - vector1.z * vector2.y;
		float yMult = vector1.z * vector2.x - vector1.x * vector2.z;
		float zMult = vector1.x * vector2.y - vector1.y * vector2.x;
		CoordsInter crossProd = new CoordsInter(xMult, yMult, zMult);
		return crossProd;
	}

	
	static public CoordsInter Lerp(CoordsInter A,  CoordsInter B, float t)
	{
		t = Mathf.Clamp(t, 0, 1);
		CoordsInter v = new CoordsInter(B.x - A.x, B.y - A.y, B.z - A.z);
		float xt = A.x + v.x * t;
		float yt = A.y + v.y * t;
		float zt = A.z + v.z * t;

		return new CoordsInter(xt, yt, zt);
	}
}
