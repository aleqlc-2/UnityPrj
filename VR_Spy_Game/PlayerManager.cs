using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            this.gameObject.transform.position = new Vector3(0, 0.56f, 0);
            PlannedAgent.instance.followTarget = null;
        }
    }
}
