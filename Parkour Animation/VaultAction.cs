using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/Custom Actions/New vault action")]
public class VaultAction : ParkourAction
{
	public override bool CheckIfPossible(ObstacleHitData hitData, Transform player)
	{
		if (!base.CheckIfPossible(hitData, player))
			return false;

		// InverseTransformPoint´Â world¸¦ local·Î
		var hitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

		if (hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0)
		{
			// Mirror Animation
			base.Mirror = true;
			base.matchBodyPart = AvatarTarget.RightHand;
		}
		else
		{
			// Not Mirror
			base.Mirror = false;
			base.matchBodyPart = AvatarTarget.LeftHand;
		}

		return true;
	}
}
