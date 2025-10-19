using UnityEngine;

public class CreateSolidWall : MonoBehaviour
{
	public Transform A;
	public Transform B;
	public Transform C;
	public Transform D;
	public Transform E;
	public GameObject ball;

	Plane wall;
	Line ballPath;
	Line trajectory;

	private void Start()
	{
		wall = new Plane(new CoordsInter(A.position), new CoordsInter(B.position), new CoordsInter(C.position));

		for (float s = 0; s <= 1; s += 0.1f)
		{
			for (float t = 0; t <= 1; t += 0.1f)
			{
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = wall.Lerp(s, t).ToVector();
			}
		}

		ballPath = new Line(new CoordsInter(D.position), new CoordsInter(E.position), Line.LINETYPE.RAY);
		ballPath.Draw(0.1f, Color.green);
		ball.transform.position = ballPath.A.ToVector();

		float intersectT = ballPath.IntersectsAt(wall);
		//if (intersectT == intersectT)
		//{
		trajectory = new Line(ballPath.A, ballPath.Lerp(intersectT), Line.LINETYPE.SEGMENT);
		//}
	}

	private void Update()
	{
		if (Time.time <= 1)
			ball.transform.position = trajectory.Lerp(Time.time).ToVector();
		else
			ball.transform.position += trajectory.Reflect(HolisticMathInter.Cross(wall.v, wall.u)).ToVector() * Time.deltaTime* 10;
	}
}
