using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroll : MonoBehaviour
{
    private float scroll_Speed = 0.1f;

    private MeshRenderer mesh_Renderer = null;

    private float x_Scroll = 0f;

    private void Awake()
    {
        mesh_Renderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        Scroll();
    }

    private void Scroll()
    {
        x_Scroll = Time.time * scroll_Speed;

        Vector2 offset = new Vector2(0f, x_Scroll);
        mesh_Renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
