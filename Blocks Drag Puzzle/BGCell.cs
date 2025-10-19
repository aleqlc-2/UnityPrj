using UnityEngine;

public class BGCell : MonoBehaviour
{
    [HideInInspector] public bool IsBlocked;
    [HideInInspector] public bool IsFilled;

    [SerializeField] private SpriteRenderer _bgSprite; // BGCellÇÁ¸®ÆéÀÇ SpriteRenderer
    [SerializeField] private Sprite _emptySprite; // grey
    [SerializeField] private Sprite _blockedSprite; // black
    [SerializeField] private Color _startColor; // Èò»ö
    [SerializeField] private Color _correctColor; // ÃÊ·Ï»ö
    [SerializeField] private Color _incorrectColor; // »¡°­»ö

    public void Init(int blockValue)
    {
        IsBlocked = blockValue == -1;
        if (IsBlocked)
        {
            IsFilled = true;
        }

        _bgSprite.sprite = IsBlocked ? _blockedSprite : _emptySprite;
    }

    public void ResetHighLight()
    {
        _bgSprite.color = _startColor;
    }

    public void UpdateHighlight(bool isCorrect)
    {
        _bgSprite.color = isCorrect ? _correctColor : _incorrectColor;
    }
}
