using UnityEngine;

public class CreateWall : MonoBehaviour
{
    Line wall;
    Line ballPath;
	Line trajectory;
    public GameObject ball;

	private void Start()
	{
		wall = new Line(new CoordsInter(5, -2, 0), new CoordsInter(0, 5, 0));
		wall.Draw(1, Color.blue);

		ballPath = new Line(new CoordsInter(-6, 0, 0), new CoordsInter(100, 0, 0));
		ballPath.Draw(0.1f, Color.yellow);

		ball.transform.position = ballPath.A.ToVector();

		float t = ballPath.IntersectsAt(wall);
		float s = wall.IntersectsAt(ballPath);
		//if (t == t && s == s)
		//{
			trajectory = new Line(ballPath.A, ballPath.Lerp(t), Line.LINETYPE.SEGMENT);
		//}
	}

	private void Update()
	{
		if (Time.time <= 1)
			ball.transform.position = trajectory.Lerp(Time.time).ToVector();
		else
			ball.transform.position += trajectory.Reflect(CoordsInter.Perp(wall.v)).ToVector() * Time.deltaTime * 5; // Time.deltaTime * 5곱해 조금 천천히 일정하게 날아가도록
	}
}
