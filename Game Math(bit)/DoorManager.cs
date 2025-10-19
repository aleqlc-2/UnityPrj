using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    int doorType = 0;

    void OnCollisionEnter(Collision collision)
    {
        if((collision.gameObject.GetComponent<AttributeManager>().attributes & doorType) != 0)
        {
            this.GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        this.GetComponent<BoxCollider>().isTrigger = false;
        other.gameObject.GetComponent<AttributeManager>().attributes &= ~doorType;
    }

    void Start()
    {
        if(this.gameObject.tag == "BLUE_DOOR") doorType = AttributeManager.BLUEKEY;
        if(this.gameObject.tag == "RED_DOOR") doorType = AttributeManager.REDKEY;
        if(this.gameObject.tag == "GREEN_DOOR") doorType = AttributeManager.GREENKEY;
        if (this.gameObject.tag == "PURPLE_DOOR") doorType = (AttributeManager.REDKEY | AttributeManager.BLUEKEY);
    }
}
