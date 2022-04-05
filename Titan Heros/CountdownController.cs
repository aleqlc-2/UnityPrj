using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    private AudioSource audioSource;

    private Animator anim;

    public GameObject countdown_3, countdown_2, countdown_1;

    public GMCameraAnimationsController cameraAnimationsController;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    public void StartCountdown()
    {
        anim.enabled = true;

        Countdown3();
    }

    // Animator탭에서 디폴트로 설정되어있으므로 anim.Play(AnimationTags.COUNTDOWN_3_ANIMATION); 안해도 실행됨
    private void Countdown3()
    {
        countdown_3.SetActive(true);
    }

    private void Countdown2()
    {
        countdown_3.SetActive(false);
        countdown_2.SetActive(true);

        anim.Play(AnimationTags.COUNTDOWN_2_ANIMATION);
    }

    private void Countdown1()
    {
        countdown_2.SetActive(false);
        countdown_1.SetActive(true);

        anim.Play(AnimationTags.COUNTDOWN_1_ANIMATION);
    }

    private void ActivateMainCamera()
    {
        countdown_1.SetActive(false);
        cameraAnimationsController.TurnOnMainCamera();
    }

    private void PlayCountdownSound()
    {
        audioSource.Play();
    }
}
