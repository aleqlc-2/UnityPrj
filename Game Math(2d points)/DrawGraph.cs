using UnityEngine;

public class DrawGraph : MonoBehaviour
{
    public int size = 20;
    public int xMax = 200;
    public int yMax = 200;

	private void Start()
	{
		Coords startPointXAxis = new Coords(xMax, 0);
		Coords endPointXAxis = new Coords(-xMax, 0);

		Coords startPointYAxis = new Coords(0, yMax);
		Coords endPointYAxis = new Coords(0, -yMax);

		Coords.DrawLine(startPointXAxis, endPointXAxis, 1, Color.red); // x축 센터
		Coords.DrawLine(startPointYAxis, endPointYAxis, 1, Color.green); // y축 센터

		// y축 격자
		int xoffset = (int)(xMax / (float)size);
		for (int x = -xoffset * size; x <= xoffset * size; x += size)
		{
			Coords.DrawLine(new Coords(x, -yMax), new Coords(x, yMax), 0.5f, Color.white);
		}

		// x축 격자
		int yoffset = (int)(yMax / (float)size);
		for (int y = -yoffset * size; y <= yoffset * size; y += size)
		{
			Coords.DrawLine(new Coords(-xMax, y), new Coords(xMax, y), 0.5f, Color.white);
		}
	}
}
