using UnityEngine;

public class Transformations : MonoBehaviour
{
    public GameObject[] points;
    public Vector3 angle;
	public Vector3 translation; // 이동할 증분값
	public Vector3 scaling; // 포지션에 곱할값
	public Vector3 shear;

	private void Start()
	{
		//angle = angle * Mathf.Deg2Rad;

		//foreach (GameObject p in points)
		//{
		//Coords position = new Coords(p.transform.position, 1);
		//p.transform.position = HolisticMath.Rotate(position, angle.x, true, angle.y, true, angle.z, true).ToVector();

		// p.transform.position = HolisticMath.Translate(position, new Coords(new Vector3(translation.x, translation.y, translation.z), 0)).ToVector();
		// p.transform.position = HolisticMath.Scale(position, scaling.x, scaling.y, scaling.z).ToVector();
		//}

		foreach (GameObject p in points)
		{
			Coords position = new Coords(p.transform.position, 1);
			p.transform.position = HolisticMath.Shear(position, shear.x, shear.y, shear.z).ToVector();
		}

		//foreach (GameObject p in points)
		//{
		//	Coords position = new Coords(p.transform.position, 1);
		//	p.transform.position = HolisticMath.Reflect(position).ToVector();
		//}

		Coords.DrawLine(new Coords(points[0].transform.position), new Coords(points[1].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[1].transform.position), new Coords(points[2].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[2].transform.position), new Coords(points[3].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[3].transform.position), new Coords(points[0].transform.position), 0.02f, Color.yellow);

		Coords.DrawLine(new Coords(points[4].transform.position), new Coords(points[5].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[5].transform.position), new Coords(points[6].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[6].transform.position), new Coords(points[7].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[7].transform.position), new Coords(points[4].transform.position), 0.02f, Color.yellow);

		Coords.DrawLine(new Coords(points[2].transform.position), new Coords(points[6].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[7].transform.position), new Coords(points[3].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[5].transform.position), new Coords(points[1].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[4].transform.position), new Coords(points[0].transform.position), 0.02f, Color.yellow);

		Coords.DrawLine(new Coords(points[8].transform.position), new Coords(points[9].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[9].transform.position), new Coords(points[2].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[9].transform.position), new Coords(points[6].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[8].transform.position), new Coords(points[3].transform.position), 0.02f, Color.yellow);
		Coords.DrawLine(new Coords(points[8].transform.position), new Coords(points[7].transform.position), 0.02f, Color.yellow);
	}
}
