using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandScript : MonoBehaviour
{
    private string RandomIslandName()
    {
        int choice = Random.Range(0, 4);
        string islandChoice = "Island2";

        switch (choice)
        {
            case 0:
                islandChoice = "Island1";
                break;

            case 1:
                islandChoice = "Island2";
                break;

            case 2:
                islandChoice = "Island3";
                break;

            case 3:
                islandChoice = "Island4";
                break;
        }

        return islandChoice + "(Clone)";
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject island = Spawner.instance.GetPooledIsland(RandomIslandName());
            island.transform.position = new Vector3(0, 0, other.transform.position.z + 4000);
            island.SetActive(true);

            gameObject.SetActive(false);
        }
    }
}
