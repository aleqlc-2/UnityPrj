using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHookTrigger : MonoBehaviour
{
    // isTrigger 체크하나 안하나 똑같이 동작하는데 왜이러지?
    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.layer == LayerMask.NameToLayer("Hook"))
        {
            PlayerController.instance.HitHook(c.transform);
            CameraController.instance.CallCameraShakeEffect();
        }

        if (c.tag == "Lava")
        {
            GameSceneScript.instance.SceneCall(1);
        }
    }
}
