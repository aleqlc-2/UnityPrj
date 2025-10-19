using UnityEngine;

public class QuaternionRotation : MonoBehaviour
{
    public GameObject[] points;
    public float angle;
    public Vector3 axis;

	private void Start()
	{
		foreach (GameObject p in points)
		{
			Coords position = new Coords(p.transform.position, 1);
			Coords ax = new Coords(axis, 0);
			p.transform.position = HolisticMath.QRotate(position, ax, angle).ToVector();
		}

		Coords.DrawLine(new Coords(0, 0, 0), new Coords(axis) * 3, 0.1f, Color.yellow);
	}
}
