using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnController : MonoBehaviour
{
    private Animator anim;

    public GameObject bossSpawnCamera;

    public EnemySpawner enemySpawner;

    public float delayBefore_SpawningBoss = 2f;
    public float delayBefore_BossFightStarts = 4f;
    public float shakeTime = 0.5f;

    private ShakeCamera shakeCamera;

    void Awake()
    {
        anim = GetComponent<Animator>();
        shakeCamera = GetComponent<ShakeCamera>();
    }

    public void StartBossSpawn()
    {
        StartCoroutine(BossSpawnWithDelay());
    }

    private IEnumerator BossSpawnWithDelay()
    {
        // WaitForSeconds는 Time.timeScale이 반영된 시간을 기다리고 WaitForSecondsRealtime은 현실 시간을 기다린다
        yield return new WaitForSecondsRealtime(delayBefore_SpawningBoss);

        // Time.timeScale = 0f으로 하여 게임이 일시중지된 동안에도 애니메이션을 적용하고 싶을 때에는
        // Boss Spawn Controller와 모든 보스프리펩의 Animator컴포넌트의 Update Mode를 Unscaled Time으로 하여
        // Animator가 Time.timeScale과 독립적으로 업데이트되도록 해야함
        Time.timeScale = 0f;

        bossSpawnCamera.SetActive(true);

        anim.Play(AnimationTags.SLIDE_IN_ANIMATION);
    }

    private void ShakeAndSpawn()
    {
        StartCoroutine(ShakeTheCameraAndSpawnTheBoss());
    }

    private IEnumerator ShakeTheCameraAndSpawnTheBoss()
    {
        shakeCamera.InitializeValues(shakeTime);

        enemySpawner.SpawnBoss(0);

        yield return new WaitForSecondsRealtime(shakeTime + delayBefore_BossFightStarts);

        // Animator컴포넌트의 Update Mode를 Unscaled Time으로 해야함
        Time.timeScale = 1f;

        bossSpawnCamera.SetActive(false);
    }
}
