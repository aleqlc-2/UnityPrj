//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class HookPoolerController : MonoBehaviour
//{
//    public Transform player;

//    [Space(3)]
//    [Header("=============== Hook Counter =================")]
//    [Space(3)]
//    public int activeHookCounter;
//    public int totalHookCount;

//    [Space(3)]
//    [Header("=============== Normal Counter =================")]
//    [Space(3)]
//    public int normalHookCount; // 몇개의 normal hook 을 생성할 것인가
//    public int enemyHookCount;
//    public Transform refNormalHook;
//    public Transform refEnemyHook;

//    [Space(3)]
//    [Header("=============== Hook Instantiate Values =================")]
//    [Space(3)]
//    public int ratePerFrame; // 프레임당 hook 생성 비율

//    [Space(3)]
//    [Header("=============== Hooks Storage =================")]
//    [Space(3)]
//    public List<Transform> hooksList; // 활성화된 hook들

//    [Space(3)]
//    [Header("=============== Hooks Distance Conditional Values =================")]
//    [Space(3)]
//    public int minDist; // 플레이어와의 최소거리
//    public int maxDist; // 플레이어와의 최대거리
//    public int farDist;

//    private void Awake()
//    {
//        totalHookCount = normalHookCount + enemyHookCount;
//    }

//    private void Start()
//    {
//        StartCoroutine(InitHooks(refNormalHook, normalHookCount));
//        StartCoroutine(InitHooks(refEnemyHook, enemyHookCount));
//        StartCoroutine(HookManager());
//    }

//    private IEnumerator InitHooks(Transform refHook, int hookCount)
//    {
//        while (hookCount > 0)
//        {
//            for (int i = 0; i < ratePerFrame && hookCount > 0; i++)
//            {
//                Transform hook = Instantiate(refHook);
//                hook.parent = transform;

//                hooksList.Add(hook);

//                ChangeHookPos(hook.GetChild(0));

//                activeHookCounter++;

//                hookCount--;
//            }
//            yield return null;
//        }
//    }

//    private void ChangeHookPos(Transform hookChild, bool isFar = false)
//    {
//        Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

//        float dist;

//        if (isFar)
//        {
//            dist = Random.Range(farDist, maxDist);
//        }
//        else
//        {
//            dist = Random.Range(minDist, maxDist);
//        }

//        Vector3 newPos = player.position + (dir * dist);

//        if (isFar && Vector3.Distance(newPos, player.position) < farDist)
//        {
//            ChangeHookPos(hookChild, isFar);
//            return;
//        }
//        else if (Physics2D.OverlapCircle(newPos, 1f, LayerMask.GetMask("Hook", "Ground")))
//        {
//            ChangeHookPos(hookChild, isFar);
//            return;
//        }

//        hookChild.parent.position = newPos;
//        hookChild.eulerAngles = Vector3.zero;
//        hookChild.parent.eulerAngles = Vector3.zero;

//    }

//    private IEnumerator HookManager()
//    {
//        yield return new WaitUntil(() => activeHookCounter == totalHookCount);
//        float dist;
//        while (true)
//        {
//            foreach (Transform hook in hooksList)
//            {
//                dist = Vector3.Distance(hook.position, player.position);
//                if (dist > maxDist)
//                {
//                    ChangeHookPos(hook.GetChild(0), true);
//                }
//            }

//            yield return null;
//        }
//    }
//}
