using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : QuantumViewComponent<CameraViewContext>
{
	private void Update()
	{
		transform.LookAt(ViewContext.VirtualCamera.transform);
	}
}
