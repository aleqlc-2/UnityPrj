using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ball에 부착된 스크립트
public class ColorChanger : MonoBehaviour
{
    void OnCollisionEnter(Collision target)
    {
        if (target.gameObject.tag == "red") // 게임오버
        {
            base.gameObject.GetComponent<Collider>().enabled = false; // ?
            target.gameObject.GetComponent<MeshRenderer>().enabled = true;
            target.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            base.GetComponent<Rigidbody>().AddForce(Vector3.down * 50, ForceMode.Impulse); // 레드조각맞추면 ball이 아래로 떨어짐
            Destroy(gameObject, 0.5f); // 볼 파괴
            SceneManager.LoadScene(0);
        }
        else
        {
            base.gameObject.GetComponent<Collider>().enabled = true; // 이미 활성화되있는데?
            GameObject gameObject = Instantiate(Resources.Load("splash1")) as GameObject; // 물감 생성
            gameObject.transform.parent = target.gameObject.transform; // 물감이 맞은곳에 붙어서 회전하도록
            Destroy(gameObject, 0.1f); // 물감 삭제
            target.gameObject.name = "color";
            target.gameObject.tag = "red"; // 맞췄던거 또 맞추면 게임오버되도록
            StartCoroutine(ChangeColor(target.gameObject));
        }
    }

    private IEnumerator ChangeColor(GameObject g)
    {
        yield return new WaitForSeconds(0.1f);
        g.gameObject.GetComponent<MeshRenderer>().enabled = true;
        g.gameObject.GetComponent<MeshRenderer>().material.color = BallHandler.oneColor; // 맞은 조각색깔을 ball의 색깔로 변경
        Destroy(base.gameObject); // 볼 삭제
    }
}
