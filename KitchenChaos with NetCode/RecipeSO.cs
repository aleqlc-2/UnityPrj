using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject // �ϼ��� ������ ��ũ���ͺ� ������Ʈ(�����Ϳ��� ����)
{
    public List<KitchenObjectSO> kitchenObjectSOList; // �� ����(�����Ϳ��� �Ҵ�)
    public string recipeName; // �ϼ��� ������ �̸�(�����Ϳ��� �Ҵ�)
}
