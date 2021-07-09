using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public float damage = 2f;
    public float radius = 1f;
    public LayerMask layerMask;

    void Update()
    {
        // 휘두를때 충돌인식 Attack Point의 반경내에 Enemy 레이어마스크를 가진 오브젝트만 배열로 넣음.
        // radius가 1f이므로 딱 Attack Point 정도의 크기임
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, layerMask);

        if (hits.Length > 0)
        {
            print("We touched: " + hits[0].gameObject.tag);
            hits[0].gameObject.GetComponent<HealthScript>().ApplyDamage(damage);
            gameObject.SetActive(false); // 이 코드 없어도 공격이 끝나면 비활성화되는데..?
        }
    }
}
