using UnityEngine;

public class CoordsInter
{
	public float x;
	public float y;
	public float z;

	public CoordsInter(float _X, float _Y)
	{
		x = _X;
		y = _Y;
		z = -1;
	}

	public CoordsInter(float _X, float _Y, float _Z)
	{
		x = _X;
		y = _Y;
		z = _Z;
	}

	public CoordsInter(Vector3 vecpos)
	{
		x = vecpos.x;
		y = vecpos.y;
		z = vecpos.z;
	}

	// normalized
	public CoordsInter GetNormal()
	{
		float magnitude = HolisticMathInter.Distance(new CoordsInter(0, 0, 0), new CoordsInter(x, y, z));
		return new CoordsInter(x / magnitude, y / magnitude, z / magnitude);
	}

	public override string ToString()
	{
		return "(" + x + "," + y + "," + z + ")";
	}

	public Vector3 ToVector()
	{
		return new Vector3(x, y, z);
	}

	static public CoordsInter operator+ (CoordsInter a, CoordsInter b)
	{
		CoordsInter c = new CoordsInter(a.x + b.x, a.y + b.y, a.z + b.z);
		return c;
	}

	static public CoordsInter operator- (CoordsInter a, CoordsInter b)
	{
		CoordsInter c = new CoordsInter(a.x - b.x, a.y - b.y, a.z - b.z);
		return c;
	}

	static public CoordsInter operator*(CoordsInter a, float b)
	{
		CoordsInter c = new CoordsInter(a.x * b, a.y * b, a.z * b);
		return c;
	}

	static public CoordsInter operator/(CoordsInter a, float b)
	{
		CoordsInter c = new CoordsInter(a.x / b, a.y / b, a.z / b);
		return c;
	}

	static public CoordsInter Perp(CoordsInter v)
	{
		return new CoordsInter(-v.y, v.x, 0); // (x,y)ÀÇ Á÷±³ º¤ÅÍ
	}

	static public void DrawLine(CoordsInter startPoint, CoordsInter endPoint, float width, Color colour)
	{
		GameObject line = new GameObject("Line_" + startPoint.ToString() + "_" + endPoint.ToString());
		LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
		lineRenderer.material.color = colour;
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, startPoint.z));
		lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, endPoint.z));
		lineRenderer.startWidth = width;
		lineRenderer.endWidth = width;
	}
}
