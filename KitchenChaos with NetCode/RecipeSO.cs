using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject // 완성된 음식의 스크립터블 오브젝트(에디터에서 생성)
{
    public List<KitchenObjectSO> kitchenObjectSOList; // 들어간 재료들(에디터에서 할당)
    public string recipeName; // 완성된 음식의 이름(에디터에서 할당)
}
