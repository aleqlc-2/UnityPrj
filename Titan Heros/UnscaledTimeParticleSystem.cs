using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Boss Spawn FX 오브젝트의 하위개체들에 부착된 스크립트
public class UnscaledTimeParticleSystem : MonoBehaviour
{
    private ParticleSystem particleFX;

    void Awake()
    {
        particleFX = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // 보스가 스폰될때 파티클 효과 보여주기위함
        if (Time.timeScale < 0.01f)
        {
            // 지정된 시간 동안 파티클을 시뮬레이션하여 파티클 시스템을 빨리 감았다가 일시 중지함
            // Time.unscaledDeltaTime는 Time.timeScale의 영향을 받지 않음
            // 두번째인자 true는 모든 자식개체도 빨리감음
            // 세번째인자 false는 재시작은 안함
            particleFX.Simulate(Time.unscaledDeltaTime, true, false);
        }
    }
}
