using System.Collections.Generic;
using UnityEngine;

public class Centipede : MonoBehaviour
{
    private List<CentipedeSegment> segments = new List<CentipedeSegment>();
    public CentipedeSegment segmentPrefab;
    public Sprite headSprite;
    public Sprite bodySprite;
	public int size = 12;
	public float speed = 20f;
	public LayerMask collisionMask;
	public BoxCollider2D homeArea;
	public Mushroom mushroomPrefab;

	private void Start()
	{
		Respawn();
	}

	private void Respawn()
	{
		foreach (CentipedeSegment segment in segments)
		{
			Destroy(segment.gameObject);	
		}

		segments.Clear();

		for (int i = 0; i < size; i++)
		{
			Vector2 position = GridPosition(transform.position) + (Vector2.left * i);
			CentipedeSegment segment = Instantiate(segmentPrefab, position, Quaternion.identity);
			segment.spriteRenderer.sprite = i == 0 ? headSprite : bodySprite;
			segment.centipede = this;
			segments.Add(segment);
		}

		for (int i = 0; i < segments.Count; i++)
		{
			CentipedeSegment segment = segments[i];
			segment.ahead = GetSegmentAt(i-1); // 앞의 segment, head는 이 값이 null
			segment.behind = GetSegmentAt(i+1); // 뒤의 segment, 마지막 segment는 이 값이 null
		}
	}

	private CentipedeSegment GetSegmentAt(int index)
	{
		if (index >= 0 && index < segments.Count)
			return segments[index];
		else
			return null;
	}

	private Vector2 GridPosition(Vector2 position)
	{
		position.x = Mathf.Round(position.x);
		position.y = Mathf.Round(position.y);
		return position;
	}

	public void Remove(CentipedeSegment segment)
	{
		Vector3 position = GridPosition(segment.transform.position);
		Instantiate(mushroomPrefab, position, Quaternion.identity);

		if (segment.ahead != null)
			segment.ahead.behind = null;

		// 잘린 뒷라인 젤 앞의 segment를 head로 바꾼다
		if (segment.behind != null)
		{
			segment.behind.ahead = null;
			segment.behind.spriteRenderer.sprite = headSprite;
			segment.behind.UpdateHeadSegment();
		}

		segments.Remove(segment);
		Destroy(segment.gameObject);
	}
}
