using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneSpawner : MonoBehaviour
{
    private GameObject player;

    private float left_BorderLimitX;
    private float right_BorderLimitX;
    private float vertical_LowerLimit;
    private float vertical_UpperLimit;

    public float planeYSpawnPos = 90f;

    private bool continueSpawning;

    public static float waitTime = 10f;

    private WaitForSeconds waitForSeconds = new WaitForSeconds(waitTime);

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        PlayerController p = player.GetComponent<PlayerController>();

        left_BorderLimitX = p.left_BorderLimitX;
        right_BorderLimitX = p.right_BorderLimitX;
        vertical_LowerLimit = p.vertical_LowerLimit;
        vertical_UpperLimit = p.vertical_UpperLimit;
    }

    void SpawnSingle(string Type)
    {
        GameObject plane = Spawner.instance.GetPooledEnemyPlane(Type + "(Clone)");

        plane.transform.position = new Vector3(player.transform.position.x, planeYSpawnPos, player.transform.position.z + 4000);

        plane.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);
        plane.SetActive(true);
    }

    void SpawnDouble(string Type)
    {
        GameObject[] planes = new GameObject[2];

        for (int i = 0; i < 2; i++)
        {
            GameObject plane = Spawner.instance.GetPooledEnemyPlane(Type + "(Clone)");
            plane.SetActive(true);
            planes[i] = plane;
        }

        int spawnChoice = Random.Range(0, 2);

        string typeOfSpawn = "";

        if (spawnChoice == 0)
            typeOfSpawn = "sidebyside";
        else
            typeOfSpawn = "separated";

        int planeOneX = (int)Random.Range(left_BorderLimitX, right_BorderLimitX - 85);
        int sideBySideOffset = 40;

        if (typeOfSpawn == "sidebyside")
        {
            if (Type == "bomber") sideBySideOffset = 85;

            planes[0].transform.position = new Vector3(planeOneX, planeYSpawnPos, player.transform.position.z + 4000);
            planes[0].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);

            planes[1].transform.position = new Vector3(planeOneX + sideBySideOffset, planeYSpawnPos, player.transform.position.z + 4000);
            planes[1].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);
        }
        else // separated
        {
            planeOneX = (int)Random.Range(left_BorderLimitX, right_BorderLimitX - left_BorderLimitX);

            sideBySideOffset = 100;

            planes[0].transform.position = new Vector3(planeOneX, planeYSpawnPos, player.transform.position.z + 4000);
            planes[0].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);

            planes[1].transform.position = new Vector3(planeOneX + sideBySideOffset, planeYSpawnPos, player.transform.position.z + 4000);
            planes[1].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);
        }
    }

    IEnumerator SpawnEnemyPlane()
    {
        while (continueSpawning)
        {
            int planeChoice = Random.Range(0, 3);
            
            string Type = "";

            if (planeChoice == 0) Type = "plane";
            else if (planeChoice == 1) Type = "bomber";
            else if (planeChoice == 2) Type = "warplane";

            int typeOfSpawn = Random.Range(0, 2);

            if (typeOfSpawn == 0) SpawnSingle(Type);
            else SpawnDouble(Type);

            yield return waitForSeconds;
        }
    }

    public void StartSpawningPlanes()
    {
        continueSpawning = true;
        StartCoroutine(SpawnEnemyPlane());
    }

    public void StopSpawningPlanes()
    {
        continueSpawning = false;
        StopCoroutine(SpawnEnemyPlane());
    }

    public void DisablePlanesMovement()
    {
        List<GameObject> planes = Spawner.instance.GetPooledEnemyPlanePool();

        foreach (var plane in planes)
        {
            if (plane.activeInHierarchy) plane.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void ResumePlanesMovement()
    {
        List<GameObject> planes = Spawner.instance.GetPooledEnemyPlanePool();

        foreach (var plane in planes)
        {
            if (plane.activeInHierarchy) plane.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);
        }
    }
}
