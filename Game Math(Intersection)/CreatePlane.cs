using UnityEngine;

public class CreatePlane : MonoBehaviour
{
    public Transform A; // 출발점
    public Transform B; // 출발점으로부터의 한방향
    public Transform C; // 출발점으로부터의 한방향
    Plane plane;

	private void Start()
	{
		plane = new Plane(new CoordsInter(A.position), new CoordsInter(B.position), new CoordsInter(C.position));
		for (float s = 0; s < 1; s+=0.1f)
		{
			for (float t = 0; t < 1; t+=0.1f)
			{
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = plane.Lerp(s, t).ToVector();
			}
		}
	}
}
