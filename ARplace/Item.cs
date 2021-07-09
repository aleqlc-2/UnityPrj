using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item1", menuName = "AddItem/Item")]
public class Item : ScriptableObject // 게임오브젝트에 부착하지않고 데이터 저장용으로 사용할 수 있는 클래스
{
    public float price;
    public GameObject itemPrefab;
    public Sprite itemImage;
}
