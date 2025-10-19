using UnityEngine;

// ��� ����� �����鿡 ������ ��ũ��Ʈ
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
