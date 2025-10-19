using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu()] // 만들고나서 에디터에 나타나지 않도록 주석처리
public class RecipeListSO : ScriptableObject
{
    public List<RecipeSO> recipeSOList; // 완성된 음식의 스크립터블을 모아놓은 리스트
}
