using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonManager : MonoBehaviour
{
    private Button btn;
    [SerializeField] private RawImage buttonImage; // 버튼 이미지

    public GameObject furniture; // AR Plane

    private Sprite _buttonTexture;
    public Sprite ButtonTexture
    {
        set
        {
            _buttonTexture = value;
            buttonImage.texture = _buttonTexture.texture;
        }
    }

    private int _itemId;
    public int itemId
    {
        set { _itemId = value; }
    }

    void Start()
    {
        btn = GetComponent<Button>();

        // 반환형이 없는 Action을 인자로 던져야하므로 SelectObject() 이렇게 쓰면 안됨
        btn.onClick.AddListener(SelectObject);
    }

    // 이 스크립트를 가지고 있는 버튼을 OnEntered메서드에 인자로 던져서
    // 수평스크롤바에서 스크롤할때 그 버튼이
    // selectionPoint위치에 있는 버튼이면 스케일이 커지도록
    void Update()
    {
        if (UIManager.Instance.OnEntered(gameObject))
        {
            //transform.localScale = Vector3.one * 2;
            transform.DOScale(Vector3.one * 2, 0.3f);
        }
        else
        {
            //transform.localScale = Vector3.one;
            transform.DOScale(Vector3.one, 0.3f);
        }
    }

    void SelectObject()
    {
       DataHandler.Instance.SetFurniture(_itemId);
    }
}