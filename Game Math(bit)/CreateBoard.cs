using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreateBoard : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public GameObject housePrefab;
    public GameObject treePrefab;
    public Text score;
    GameObject[] tiles;
    long dirtBB = 0;
    long desertBB = 0;
    long treeBB = 0;
    long playerBB = 0;

    void Start()
    {
        tiles = new GameObject[64];

        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                int randomTile = UnityEngine.Random.Range(0, tilePrefabs.Length);
                Vector3 pos = new Vector3(c, 0, r);
                GameObject tile = Instantiate(tilePrefabs[randomTile], pos, Quaternion.identity);
                tile.name = tile.tag + "_" + r + "_" + c;
                tiles[r * 8 + c] = tile;

                if (tile.tag == "Dirt")
                {
                    dirtBB = SetCellState(dirtBB, r, c);
                    // PrintBB("Dirt", dirtBB);
				}
				else if (tile.tag == "Desert")
				{
					desertBB = SetCellState(desertBB, r, c);
				}
			}
        }

        Debug.Log("Dirt cells = " + CellCount(dirtBB));
        InvokeRepeating("PlantTree", 1, 1);
    }

    void PlantTree()
    {
        int rr = UnityEngine.Random.Range(0, 8);
        int rc = UnityEngine.Random.Range(0, 8);
        if (GetCellState(dirtBB & ~playerBB, rr, rc)) // house가 없는 dirt타일에만 tree생성되도록
        {
			GameObject tree = Instantiate(treePrefab);
            tree.transform.parent = tiles[rr * 8 + rc].transform;
            tree.transform.localPosition = Vector3.zero;
            treeBB = SetCellState(treeBB, rr, rc);
        }
    }

    void PrintBB(string name, long BB)
    {
        Debug.Log(name + ": " + Convert.ToString(BB, 2).PadLeft(64, '0'));
    }

    long SetCellState(long bitboard, int row, int col)
    {
        long newBit = 1L << (row * 8 + col);
        return bitboard |= newBit;
    }

    bool GetCellState(long bitboard, int row, int col)
    {
        long mask = 1L << (row * 8 + col);
        return ((bitboard & mask) != 0);
    }

    // dirt cell 개수 세기
    int CellCount(long bitboard)
    {
        int count = 0;
        long bb = bitboard;
        while (bb != 0)
        {
            bb &= bb - 1; // -1한값과 -1하기전의값을 이진수로 and연산하면 -1한 값이됨
            count++;
		}

        return count;
    }

    void CalculateScore()
    {
        // dirt타일에 house만들면 10점, desert타일에 house만들면 2점
        score.text = "Score : " + (CellCount(dirtBB & playerBB) * 10 + CellCount(desertBB & playerBB) * 2);
    }

	private void Update()
	{
        // 클릭하는 타일에 house 생성
		if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                int r = (int)hit.collider.gameObject.transform.position.z;
                int c = (int)hit.collider.gameObject.transform.position.x;
                if (GetCellState((dirtBB & ~treeBB) | desertBB, r, c)) // tree가 없는 dirt나 desert타일에만 house 생성가능
                {
					GameObject house = Instantiate(housePrefab);
					house.transform.parent = hit.collider.gameObject.transform;
					house.transform.localPosition = Vector3.zero;
					playerBB = SetCellState(playerBB, (int)hit.collider.gameObject.transform.position.z, (int)hit.collider.gameObject.transform.position.x);
					CalculateScore();
				}
			}
        }
	}
}
