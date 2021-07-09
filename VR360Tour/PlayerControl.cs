using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public void GotoS1()
    {
        // transform.Translate는 현재위치에서 얼마만큼 이동하느냐의 증분값임
        // -30, 0, 0 으로 이동하는게 아니라 현재위치에서 -30, 0, 0만큼 이동하라는 뜻.
        // transform.Translate(-30, 0, 0);

        transform.position = new Vector3(0, 0, -10); // 0, 0, -10 '으로' 이동하라는 뜻.
    }

    public void GotoS2()
    {
        //transform.Translate(30, 0, 0);

        transform.position = new Vector3(30, 0, -10);
    }

    public void GotoS3()
    {
        transform.position = new Vector3(60, 0, -10);
    }
}
