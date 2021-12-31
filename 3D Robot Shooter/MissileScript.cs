using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    public GameObject explosion;

    private void LaunchMissile(Vector3 tarPos) // private여도 send받는듯
    {
        Invoke("SetActive", 0.9f);

        // LeanTweenType.easeInBack는 미사일이 살짝 뒤로갔다가 앞으로 발사되도록
        LeanTween.move(gameObject, tarPos, 1.6f).setEase(LeanTweenType.easeInBack).setOnComplete(Explode);
    }

    private void SetActive()
    {
        GetComponent<Collider>().enabled = true;
    }
    
    private void Explode() // 폭발 효과
    {
        Instantiate(explosion, transform.position, Quaternion.identity); // explosion프리펩에 폭발효과 파티클이 없음..
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (target.tag == "Box")
            {
                target.GetComponent<Rigidbody>().AddExplosionForce(2000f, transform.position, 6f); // 폭발력,힘이전달받는위치,반경,(위로솟구치는힘)
            }
            else if (target.tag == "Enemy")
            {
                target.SendMessage("Damage"); // target 오브젝트에 달린 스크립트의 Damage메서드를 호출
            }

            Explode();
        }
    }
}
