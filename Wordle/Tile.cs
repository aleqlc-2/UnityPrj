using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Tile : MonoBehaviour
{
	[Serializable] // Board스크립트에서 인스턴스화한 변수들 멤버변수와 함께 에디터에 보여줌
	public class State
	{
		// 에디터에서 할당
		public Color fillColor;
		public Color outlineColor;
	}

	public State state { get; private set; }
	private Image fill;
	private Outline outline;

    private TextMeshProUGUI text;
    public char letter { get; private set; }

	private void Awake()
	{
		text = GetComponentInChildren<TextMeshProUGUI>();
		fill = GetComponent<Image>();
		outline = GetComponent<Outline>();
	}

	public void Setletter(char letter)
	{
		this.letter = letter;
		text.text = letter.ToString();
	}

	public void SetState(State state)
	{
		this.state = state;
		fill.color = state.fillColor;
		outline.effectColor = state.outlineColor;
	}
}
