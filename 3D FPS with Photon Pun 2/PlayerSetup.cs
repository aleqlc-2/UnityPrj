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

	// 본인이면 손만보이고 적은 솔저가 보이게
	private void Start()
	{
		playerMoveController = GetComponent<PlayerMoveController>();

		if (photonView.IsMine) // 핸드 활성화, 솔저 비활성화
		{
			foreach (GameObject gameObject in FPS_Hands_ChildGameobjects)
			{
				gameObject.SetActive(true);
			}

			foreach (GameObject gameObject in Soldier_ChildGameobjects)
			{
				gameObject.SetActive(false);
			}

			// 플레이어 UI 생성
			GameObject playerUIGameobject = Instantiate(playerUIPrefab);
			playerMoveController.joystick = playerUIGameobject.transform.Find("Fixed Joystick").GetComponent<Joystick>();

			FPSCamera.enabled = true; // 본인카메라 활성화
			animator.SetBool("IsSoldier", false); // 본인은 핸드 애니메이션
		}
		else // 핸드 비활성화, 솔저 활성화
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

			FPSCamera.enabled = false; // 다른플레이어 카메라 비활성화
			animator.SetBool("IsSoldier", true); // 다른 플레이어는 솔저 전체 애니메이션
		}
	}
}
