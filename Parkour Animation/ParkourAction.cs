using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New parkour action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] private string animName;
    [SerializeField] private string obstacleTag;

	public string AnimName => animName;

	[SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;

    [SerializeField] private bool rotateToObstacle;
    public bool RotateToObstacle => rotateToObstacle;

    [SerializeField] private float postActionDelay;
    public float PostActionDelay => postActionDelay;

    [Header("Target Matching")]
    [SerializeField] private bool enableTargetMatching = true;
    [SerializeField] protected AvatarTarget matchBodyPart; // 자식클래스에서 접근가능하게 protected로
    [SerializeField] private float matchStartTime;
    [SerializeField] private float matchTargetTime;
    [SerializeField] private Vector3 matchPosWeight = new Vector3(0, 1, 1);
	public bool EnableTargetMatching => enableTargetMatching;
	public AvatarTarget MatchBodyPart => matchBodyPart;
	public float MatchStartTime => matchStartTime;
	public float MatchTargetTime => matchTargetTime;
	public Vector3 MatchPosWeight => matchPosWeight;

    public Vector3 MatchPos { get; set; }

	public Quaternion TargetRotation { get; set; }
    public bool Mirror { get; set; }
	

    public virtual bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        if (!string.IsNullOrEmpty(obstacleTag) && hitData.forwardHit.transform.tag != obstacleTag) return false;

        float height = hitData.heightHit.point.y - player.position.y;
        if (height < minHeight || height > maxHeight)
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (enableTargetMatching)
            MatchPos = hitData.heightHit.point;

        return true;
    }
}
