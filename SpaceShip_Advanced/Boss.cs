using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public enum BossFire
	{
        INORDER,
        INREVERSE,
        ROTATE,
        ALL,
        BIGBULLET
	}

    private BossFire bossFire;

    [SerializeField] private BossHealth bossHealth = null;

    [SerializeField] private GameObject rotateSpawnPoint = null;
    [SerializeField] private Transform[] childSpawnPoints = null;
    [SerializeField] private BigBullet bigBullet = null;

    List<GameObject> objList = new List<GameObject>();

    private int chance = 0;

	private void Start()
	{
        bossFire = BossFire.INORDER;
	}

	public IEnumerator fire() // GAMEMANAGER에서 3스테이지되었음을 통보받아 EVENT로 FIRE를 실행하게 구현하는게 좋음
    {
        while (true)
        {
            switch(bossFire)
			{
                case BossFire.INORDER:
                    yield return StartCoroutine(FireInOrder());
                    StopCoroutine(FireInOrder());
                    ObjPoolManager.Instance.ReturnStraightBullet(objList);
                    changeBossFire((int)bossFire + 1);
                    break;

                case BossFire.INREVERSE:
                    yield return StartCoroutine(FireInReverse());
                    StopCoroutine(FireInReverse());
                    ObjPoolManager.Instance.ReturnStraightBullet(objList);
                    changeBossFire((int)bossFire + 1);
                    break;

                case BossFire.ROTATE:
                    rotateSpawnPoint.SetActive(true);
                    yield return new WaitForSeconds(8f);
                    rotateSpawnPoint.SetActive(false);
                    yield return null;
                    changeBossFire((int)bossFire + 1);
                    break;

                case BossFire.ALL:
                    fireAll();
                    yield return new WaitForSeconds(2f);
                    ObjPoolManager.Instance.ReturnStraightBullet(objList);
                    yield return null;
                    changeBossFire((int)bossFire + 1);
                    break;

                case BossFire.BIGBULLET:
                    bigBulletFire();
                    yield return new WaitForSeconds(1f);
                    changeBossFire((int)bossFire + 1);
                    break;
            }
        }
    }

    private void changeBossFire(int stateNum)
	{
        if (stateNum == System.Enum.GetValues(typeof(BossFire)).Length)
		{
            stateNum = 0;
		}

		bossFire = (BossFire)stateNum;
    }

    private IEnumerator FireInOrder()
	{
        for (int i = 1; i < childSpawnPoints.Length; i++)
        {
            yield return new WaitForSeconds(0.3f);

            var obj = ObjPoolManager.Instance.GetStrightBullet();
            obj.transform.position = childSpawnPoints[i].position;
            obj.transform.rotation = Quaternion.identity;
            obj.GetComponent<BulletScript>().is_EnemyBullet = true;

            objList.Add(obj);

            if (bossHealth.Bosshealth <= 0) yield break;
        }
    }

    private IEnumerator FireInReverse()
    {
        for (int i = childSpawnPoints.Length - 1; i > 0; i--)
        {
            yield return new WaitForSeconds(0.3f);
            var obj = ObjPoolManager.Instance.GetStrightBullet();
            obj.transform.position = childSpawnPoints[i].position;
            obj.transform.rotation = Quaternion.identity;
            obj.GetComponent<BulletScript>().is_EnemyBullet = true;

            objList.Add(obj);

            if (bossHealth.Bosshealth <= 0) yield break;
        }
    }

    private void fireAll()
    {
        for (int i = 0; i < childSpawnPoints.Length; i++)
        {
            var obj = ObjPoolManager.Instance.GetStrightBullet();
            obj.transform.position = childSpawnPoints[i].position;
            obj.transform.rotation = Quaternion.identity;
            obj.GetComponent<BulletScript>().is_EnemyBullet = true;

            objList.Add(obj);
        }
    }

    private void bigBulletFire()
    {
        chance = Random.Range(0, 4);
        if (chance == 1 || chance == 2 || chance == 3)
        {
            bigBullet.isFire = true;
        }
    }

    //  private int test = 0;
    //  private string testStr = string.Empty;

    //  private enum BossModel { A = 0, B, C, D, E}

    //  [SerializeField] private BossModel m_BossModel = BossModel.A;
    //  [SerializeField] private GameObject[] m_BossModels = null;

    //  IEnumerator Start()
    //  {
    //      Debug.Log("asd");
    //      yield return new WaitForSeconds(3f);
    //      Debug.Log("qqq");
    //      yield return new WaitUntil(() => test >= 10); // test값이 10이 넘으면 다음 로직으로 넘어감

    //      for(int i = 0; i < m_BossModels.Length; i++)
    //{
    //          if (testStr == "kkk")
    //	{
    //              yield break; // 일반메서드의 return처럼 코루틴을 즉시 종료
    //	}
    //          m_BossModels[i].SetActive((int)m_BossModel == i);
    //      }
    //  }
}
