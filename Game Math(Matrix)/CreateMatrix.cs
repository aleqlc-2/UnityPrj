using UnityEngine;

public class CreateMatrix : MonoBehaviour
{
	private void Start()
	{
		float[] mvals = { 1, 2, 3, 4, 5, 6 };
		Matrix m = new Matrix(2, 3, mvals);

		float[] nvals = { 1, 2, 3, 4, 5, 6 };
		Matrix n = new Matrix(3, 2, nvals);

		Matrix answer = m * n;

		Debug.Log(m.ToString() + "\n" + n.ToString() + "\n" + answer.ToString());
	}
}
