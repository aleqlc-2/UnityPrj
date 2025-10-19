using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParkourController : MonoBehaviour
{
	[SerializeField] private List<ParkourAction> parkourActions;
	[SerializeField] private ParkourAction jumpDownAction;
	[SerializeField] private float autoDropHeightLimit = 1f;

	private bool inAction;

    private EnvironmentScanner environmentScanner;
	private Animator animator;
	private PlayerController playerController;
	
	private void Awake()
	{
		environmentScanner = GetComponent<EnvironmentScanner>();
		animator = GetComponent<Animator>();
		playerController = GetComponent<PlayerController>();
	}

	private void Update()
	{
		var hitData = environmentScanner.ObstacleCheck();

		if (Input.GetButton("Jump") && !playerController.InAction && !playerController.IsHanging)
		{
			if (hitData.forwardHitFound)
			{
				Debug.Log("Obstacle Found " + hitData.forwardHit.transform.name);

				foreach (var action in parkourActions)
				{
					if (action.CheckIfPossible(hitData, transform))
					{
						StartCoroutine(DoParkourAction(action));
						break;
					}
				}
			}
		}

		if (playerController.IsOnLedge && !playerController.InAction && !hitData.forwardHitFound)
		{
			bool shouldJump = true;
			if (playerController.LedgeData.height > autoDropHeightLimit && !Input.GetButton("Jump")) shouldJump = false;

			if (shouldJump && playerController.LedgeData.angle <= 50)
			{
				playerController.IsOnLedge = false;
				StartCoroutine(DoParkourAction(jumpDownAction));
			}
		}
	}

	private IEnumerator DoParkourAction(ParkourAction action)
	{
		playerController.SetControl(false);

		MatchTargetParams matchParams = null;
		if (action.EnableTargetMatching)
		{
			matchParams = new MatchTargetParams()
			{
				pos = action.MatchPos,
				bodyPart = action.MatchBodyPart,
				posWeight = action.MatchPosWeight,
				startTime = action.MatchStartTime,
				targetTime = action.MatchTargetTime
			};
		}

		yield return playerController.DoAction(action.AnimName, matchParams, action.TargetRotation, action.RotateToObstacle, action.PostActionDelay, action.Mirror);
		playerController.SetControl(true);
	}

	private void MatchTarget(ParkourAction action)
	{
		if (animator.isMatchingTarget) return; // 자동매치 된다면 리턴
		
		// 자동매치 안되면 직접매치
		animator.MatchTarget(
			action.MatchPos,
			transform.rotation,
			action.MatchBodyPart,
			new MatchTargetWeightMask(action.MatchPosWeight, 0), // 파쿠르대상과 겹치지 않도록 비중 조절
			action.MatchStartTime,
			action.MatchTargetTime);
	}
}
