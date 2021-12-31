using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public Image fillWaitImage_1;
    public Image fillWaitImage_2;
    public Image fillWaitImage_3;
    public Image fillWaitImage_4;
    public Image fillWaitImage_5;
    public Image fillWaitImage_6;

    private int[] fadeImages = new int[] { 0, 0, 0, 0, 0, 0 };

    private Animator anim;

    private bool canAttack = true;

    private PlayerMove playerMove;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Stand")) // base layer일때만 스킬을 누를수 있고
            canAttack = true;
        else // 스킬을 눌러 다른 애니메이션이 진행되는 중에는 다른 스킬을 눌러도 반응이 없도록
            canAttack = false;

        CheckToFade();
        CheckInput();
    }

    private void CheckToFade()
    {
        if (fadeImages[0] == 1)
        {
            if (FadeAndWait(fillWaitImage_1, 1.0f)) fadeImages[0] = 0;
        }

        if (fadeImages[1] == 1)
        {
            if (FadeAndWait(fillWaitImage_2, 0.7f)) fadeImages[1] = 0;
        }

        if (fadeImages[2] == 1)
        {
            if (FadeAndWait(fillWaitImage_3, 0.1f)) fadeImages[2] = 0;
        }

        if (fadeImages[3] == 1)
        {
            if (FadeAndWait(fillWaitImage_4, 0.2f)) fadeImages[3] = 0;
        }

        if (fadeImages[4] == 1)
        {
            if (FadeAndWait(fillWaitImage_5, 0.3f)) fadeImages[4] = 0;
        }

        if (fadeImages[5] == 1)
        {
            if (FadeAndWait(fillWaitImage_6, 0.08f)) fadeImages[5] = 0;
        }
    }

    // fadeImg은 스킬이미지 위에 비활성화된채로 있는 어두운이미지, fadeTime이 쿨타임
    private bool FadeAndWait(Image fadeImg, float fadeTime)
    {
        bool faded = false;

        if (fadeImg == null) return faded;

        if (!fadeImg.gameObject.activeInHierarchy)
        {
            fadeImg.gameObject.SetActive(true);
            fadeImg.fillAmount = 1f; // 1이 어둡게 가려진거
        }

        fadeImg.fillAmount -= fadeTime * Time.deltaTime; // 쿨타임 진행중(점점 밝아짐)

        if (fadeImg.fillAmount <= 0.0f) // 쿨타임 끝나면
        {
            fadeImg.gameObject.SetActive(false);
            faded = true;
        }

        return faded;
    }

    private void CheckInput()
    {
        if (anim.GetInteger("Atk") == 0)
        {
            playerMove.FinishedMovement = false;

            if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Stand"))
            {
                playerMove.FinishedMovement = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 누르면
        {
            if (playerMove.FinishedMovement && fadeImages[0] != 1 && canAttack)
            {
                fadeImages[0] = 1;
                anim.SetInteger("Atk", 1);
                playerMove.TargetPosition = transform.position;
                RemoveCursorPoint();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 2 누르면
        {
            if (playerMove.FinishedMovement && fadeImages[1] != 1 && canAttack)
            {
                fadeImages[1] = 1;
                anim.SetInteger("Atk", 2);
                playerMove.TargetPosition = transform.position;
                RemoveCursorPoint();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // 3 누르면
        {
            if (playerMove.FinishedMovement && fadeImages[2] != 1 && canAttack)
            {
                fadeImages[2] = 1;
                anim.SetInteger("Atk", 3);
                playerMove.TargetPosition = transform.position;
                RemoveCursorPoint();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // 4 누르면
        {
            if (playerMove.FinishedMovement && fadeImages[3] != 1 && canAttack)
            {
                fadeImages[3] = 1;
                anim.SetInteger("Atk", 4);
                playerMove.TargetPosition = transform.position;
                RemoveCursorPoint();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) // 5 누르면
        {
            if (playerMove.FinishedMovement && fadeImages[4] != 1 && canAttack)
            {
                fadeImages[4] = 1;
                anim.SetInteger("Atk", 5);
                playerMove.TargetPosition = transform.position;
                RemoveCursorPoint();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (playerMove.FinishedMovement && fadeImages[5] != 1 && canAttack)
            {
                fadeImages[5] = 1;
                anim.SetInteger("Atk", 6);
                playerMove.TargetPosition = transform.position;
                RemoveCursorPoint();
            }
        }
        else
        {
            anim.SetInteger("Atk", 0);
        }

        // 스페이스 누른상태에서 마우스 돌리면 마우스포인터쪽을 캐릭터가 바라봄
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 targetPos = Vector3.zero;

            // Input.mousePosition은 클릭과 관계없는 마우스포지션임
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                targetPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(targetPos - transform.position),
                                                  15f * Time.deltaTime);
        }
    }

    // 포인트 찍고 이동중에 스킬사용하면 포인트 사라지도록
    private void RemoveCursorPoint()
    {
        GameObject cursorObj = GameObject.FindGameObjectWithTag("Cursor");
        if (cursorObj) Destroy(cursorObj); // 포인트 안찍고 스킬 사용하면 널참조 뜨므로 if문 작성
    }
}
