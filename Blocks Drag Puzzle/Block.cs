using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector3 CurrentPos => new Vector3(currentPos.x, currentPos.y, 0);

    [SerializeField] private SpriteRenderer _blockPrefab; // BlockCell 프리펩의 SpriteRenderer
    [SerializeField] private List<Sprite> _blockSprites; // 8가지 색상 타일의 Sprite
    [SerializeField] private float _blockSpawnSize; // 0.5

    private Vector3 startPos;
    private Vector3 previousPos;
    private Vector3 currentPos;
    private List<SpriteRenderer> blockSprites;
    private List<Vector2Int> blockPositions;

    private const int TOP = 1;
    private const int BOTTOM = 0;

    public void Init(List<Vector2Int> blocks, Vector3 start, int blockNum)
    {
        startPos = start;
        previousPos = start;
        currentPos = start;
        blockPositions = blocks;
        blockSprites = new List<SpriteRenderer>();

        // Block의 자식개체인 BlockCell들 생성
        for (int i = 0; i < blockPositions.Count; i++)
        {
            SpriteRenderer spawnedBlock = Instantiate(_blockPrefab, transform); // SpriteRenderer를 Instantiate했는데 오브젝트가 생성됨
			spawnedBlock.sprite = _blockSprites[blockNum + 1]; // Id에 +1해서 리스트에서 해당 인덱스 Sprite가져옴
            spawnedBlock.transform.localPosition = new Vector3(blockPositions[i].y, blockPositions[i].x, 0); // SpriteRenderer.transform.localPosition인데 위치가 바뀜
			blockSprites.Add(spawnedBlock); // 자식개체인 BlockCell들을 지역변수 List에 담는다
		}

        transform.localScale = Vector3.one * _blockSpawnSize;

        ElevateSprites(true);
    }

    public void ElevateSprites(bool reverse = false)
    {
        foreach (var blockSprite in blockSprites)
        {
            blockSprite.sortingOrder = reverse ? BOTTOM : TOP;
        }
    }

    public void UpdatePos(Vector3 offset)
    {
        currentPos += offset;
        transform.position = currentPos;
    }

    public List<Vector2Int> BlockPositions()
    {
        List<Vector2Int> result = new List<Vector2Int>();
        
        foreach (var pos in blockPositions)
        {
            // 에디터에서 블럭 1개당 할당된 3개의 start포지션에 블럭의 현재포지션을 더한다
            result.Add(pos + new Vector2Int(Mathf.FloorToInt(currentPos.y), Mathf.FloorToInt(currentPos.x)));
        }

        return result;
    }

    public void UpdateIncorrectMove()
    {
        currentPos = previousPos;
        transform.position = currentPos;
    }

    public void UpdateStartMove()
    {
        currentPos = startPos;
        previousPos = startPos;
        transform.position = currentPos;
    }

    public void UpdateCorrectMove()
    {
        currentPos.x = Mathf.FloorToInt(currentPos.x) + 0.5f;
        currentPos.y = Mathf.FloorToInt(currentPos.y) + 0.5f;
        previousPos = currentPos;
        transform.position = currentPos;
    }
}
