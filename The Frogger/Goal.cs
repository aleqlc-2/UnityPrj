using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    private GameObject frogPlayer;
    private Vector3 playerInitialPos;

    public Text scoreText;
    private int score;

    void Awake()
    {
        frogPlayer = GameObject.FindWithTag("Player");
        playerInitialPos = frogPlayer.transform.position;
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "Player")
        {
            target.transform.position = playerInitialPos;
            score += 100;
            scoreText.text = score.ToString();
        }
    }
}
