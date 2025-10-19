using UnityEngine;

public class Move : MonoBehaviour
{
    public Transform start;
    public Transform end;
    //Line line;

	private void Start()
	{
		//line = new Line(new CoordsInter(start.position), new CoordsInter(end.position), Line.LINETYPE.SEGMENT);
	}

	private void Update()
	{
		this.transform.position = HolisticMathInter.Lerp(new CoordsInter(start.position), new CoordsInter(end.position), Time.time * 0.01f).ToVector();
	}
}
