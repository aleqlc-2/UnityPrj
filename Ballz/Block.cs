using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private int hitsRemaining = 5;

    private SpriteRenderer spriteRenderer;
    private TMPro.TextMeshPro text;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        text = GetComponentInChildren<TMPro.TextMeshPro>();
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        text.SetText(hitsRemaining.ToString()); // SetText는 TMPro.TextMeshPro
        spriteRenderer.color = Color.Lerp(Color.white, Color.red, hitsRemaining / 10f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hitsRemaining--;

        if (hitsRemaining > 0)
            UpdateVisualState();
        else
            Destroy(gameObject);
    }

    internal void SetHits(int hits)
    {
        hitsRemaining = hits;
        UpdateVisualState();
    }
}