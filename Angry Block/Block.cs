using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    private int count;

    public Text countText;

    private AudioSource bounceSound;

    void Awake()
    {
        bounceSound = GameObject.Find("BounceSound").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (transform.position.y <= -10) Destroy(gameObject);
    }

    public void SetStartingCount(int count)
    {
        this.count = count;
        countText.text = count.ToString();
    }

    void OnCollisionEnter2D(Collision2D target)
    {
        if (target.collider.name == "Ball" && count > 0)
        {
            count--;
            Camera.main.GetComponent<CameraTransition>().Shake();
            countText.text = count.ToString();
            bounceSound.Play();

            if (count == 0)
            {
                Destroy(gameObject);
                Camera.main.GetComponent<CameraTransition>().MediumShake();
                GameObject.Find("ExtraBallProgress").GetComponent<Progress>().IncreaseCurrentWidth();
            }
        }
    }
}
