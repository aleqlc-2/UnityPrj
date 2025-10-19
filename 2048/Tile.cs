using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

public class Tile : MonoBehaviour
{
    public TileState state {  get; private set; }
    public TileCell cell { get; private set; }
    public int number { get; private set; }
    public bool locked { get; set; }
    
    private Image background;
    private TextMeshProUGUI text;

	private void Awake()
	{
		background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
	}

	public void SetState(TileState state, int number)
    {
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();
    }

    public void Spawn(TileCell cell)
    {
        if (this.cell != null) // 이 tile이 어떤 cell에 이미 위치해있다면
        {
            this.cell.tile = null; // 그 cell을 비워주고
        }

        // 새로운 cell로 할당된다
        this.cell = cell;
        this.cell.tile = this;

        // tile이 새로운 cell의 위치로 이동
        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
		if (this.cell != null) // 이 tile이 어떤 cell에 이미 위치해있다면
		{
			this.cell.tile = null; // 그 cell을 비워주고
		}

		// 새로운 cell로 할당된다
		this.cell = cell;
		this.cell.tile = this; // 이때 occupied가 true가 되는것

        // tile이 새로운 cell의 위치로 이동
        StartCoroutine(Animate(cell.transform.position, false));
	}

	public void Merge(TileCell cell)
	{
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        cell.tile.locked = true; // 한번 합쳐진 타일은 locked상태가되어 뒤에 따라오는 같은 숫자가 있더라도 일단 안합쳐지도록
		StartCoroutine(Animate(cell.transform.position, true));
	}

	private IEnumerator Animate(Vector3 to, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (merging)
        {
            Destroy(this.gameObject);
        }
    }
}
