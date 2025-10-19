using UnityEngine;

// 모든 재료의 프리펩에 부착된 스크립트
public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

	private void LateUpdate()
	{
        if (targetTransform == null) return;

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
	}
}
