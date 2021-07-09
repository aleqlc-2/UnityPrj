using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishedLevel : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    [SerializeField] private float timer = 2f;

    private bool levelFinished;
    private PlatformSoundFX soundFX;

    void Awake()
    {
        soundFX = GetComponent<PlatformSoundFX>();
    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.CompareTag(Tags.PLAYER_TAG))
        {
            if (!levelFinished)
            {
                levelFinished = true;
                soundFX.PlayAudio(true);

                if (!nextLevelName.Equals("")) // 다음단계가 있다면
                {
                    Invoke("LoadNewLevel", timer);
                }
            }
        }
    }

    void LoadNewLevel()
    {
        SceneManager.LoadScene(nextLevelName); // 프로젝트창에 실제 씬개체의 이름
    }
}
