using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMoveController : MonoBehaviour
{
    public Joystick joystick;

    private RigidbodyFirstPersonController rigidBodyController;

    private void Start()
    {
		rigidBodyController = GetComponent<RigidbodyFirstPersonController>();

	}

	private void FixedUpdate()
	{
		rigidBodyController.joystickInputAxis.x = joystick.Horizontal;
		rigidBodyController.joystickInputAxis.y = joystick.Vertical;
	}
}
