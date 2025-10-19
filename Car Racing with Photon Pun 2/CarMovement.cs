using UnityEngine;

public class CarMovement : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 thrustForce = new Vector3(0f, 0f, 45f);
    public Vector3 rotationTorque = new Vector3(0f, 8f, 0f);

	public bool controlsEnabled;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		controlsEnabled = false;
	}

	private void Update()
	{
		if (controlsEnabled)
		{
			if (Input.GetKey("w"))
			{
				rb.AddRelativeForce(thrustForce);
			}

			if (Input.GetKey("s"))
			{
				rb.AddRelativeForce(thrustForce);
			}

			if (Input.GetKey("a"))
			{
				rb.AddRelativeForce(thrustForce);
			}

			if (Input.GetKey("d"))
			{
				rb.AddRelativeForce(thrustForce);
			}
		}
	}
}
