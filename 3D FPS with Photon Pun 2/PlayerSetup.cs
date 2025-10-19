using Photon.Pun;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
	public GameObject[] FPS_Hands_ChildGameobjects;
	public GameObject[] Soldier_ChildGameobjects;

	public GameObject playerUIPrefab;
	private PlayerMoveController playerMoveController;

	public Camera FPSCamera;
	public Animator animator;

	// �����̸� �ո����̰� ���� ������ ���̰�
	private void Start()
	{
		playerMoveController = GetComponent<PlayerMoveController>();

		if (photonView.IsMine) // �ڵ� Ȱ��ȭ, ���� ��Ȱ��ȭ
		{
			foreach (GameObject gameObject in FPS_Hands_ChildGameobjects)
			{
				gameObject.SetActive(true);
			}

			foreach (GameObject gameObject in Soldier_ChildGameobjects)
			{
				gameObject.SetActive(false);
			}

			// �÷��̾� UI ����
			GameObject playerUIGameobject = Instantiate(playerUIPrefab);
			playerMoveController.joystick = playerUIGameobject.transform.Find("Fixed Joystick").GetComponent<Joystick>();

			FPSCamera.enabled = true; // ����ī�޶� Ȱ��ȭ
			animator.SetBool("IsSoldier", false); // ������ �ڵ� �ִϸ��̼�
		}
		else // �ڵ� ��Ȱ��ȭ, ���� Ȱ��ȭ
		{
			foreach (GameObject gameObject in FPS_Hands_ChildGameobjects)
			{
				gameObject.SetActive(false);
			}

			foreach (GameObject gameObject in Soldier_ChildGameobjects)
			{
				gameObject.SetActive(true);
			}

			playerMoveController.enabled = false;
			GetComponent<RigidbodyFirstPersonController>().enabled = false;

			FPSCamera.enabled = false; // �ٸ��÷��̾� ī�޶� ��Ȱ��ȭ
			animator.SetBool("IsSoldier", true); // �ٸ� �÷��̾�� ���� ��ü �ִϸ��̼�
		}
	}
}
