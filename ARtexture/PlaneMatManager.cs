using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneMatManager : MonoBehaviour
{
    public Material planeMat;

    public Button[] planeTexButtons;

    void Awake()
    {
        foreach (var b in planeTexButtons)
        {
            Texture tex = b.transform.Find("Mask/RawImage").GetComponent<RawImage>().texture;
            b.onClick.AddListener(() => OnClickButton(tex));
        }
    }

    void OnClickButton(Texture tex)
    {
        planeMat.mainTexture = tex;
    }
}
