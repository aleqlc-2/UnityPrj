using UnityEngine;

public class CreatePlaneHit : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public Transform C;
    public Transform D;
    public Transform E;

    Plane plane;
    Line L1;

	private void Start()
	{
        plane = new Plane(new CoordsInter(A.position), new CoordsInter(B.position), new CoordsInter(C.position));
        L1 = new Line(new CoordsInter(D.position), new CoordsInter(E.position), Line.LINETYPE.RAY);
        L1.Draw(1, Color.green);

        for (float s = 0; s <= 1; s+=0.1f)
        {
            for (float t = 0; t <= 1; t+=0.1f)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = plane.Lerp(s, t).ToVector();
            }
        }

        float interceptT = L1.IntersectsAt(plane);
        //if (interceptT == interceptT)
        //{
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = L1.Lerp(interceptT).ToVector();
        //}
	}
}
