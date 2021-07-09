using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Medusa : MonoBehaviour
{
    public GameObject score;

    public Transform playerEyeTransform;
    public Transform medusaEyeTransform;
    public Camera playerCamera;

    private float xPadding = 200f;
    private float yPadding = 200f;

    private bool isAlive;

    void Start()
    {
        isAlive = true;
    }

    void Update()
    {
        if (isAlive)
        {
            // 플레이어가 위아래로 보는건 medusa가 따라하지않도록
            transform.LookAt(
                new Vector3(playerEyeTransform.position.x, transform.position.y, playerEyeTransform.position.z));

            bool isPlayerDead = IsMedusaOnScreen() && IsPlayerInLineOfSight();
            if (isPlayerDead)
            {
                Vector3 medusaGazeDirection = playerEyeTransform.position - medusaEyeTransform.position;
                Debug.DrawRay(medusaEyeTransform.position, medusaGazeDirection, Color.red, 0.5f);
            }
        }
    }

    bool IsMedusaOnScreen()
    {
        // convert Medusa eye point to player camera's screen space
        Vector3 medusaPointInScreenSpace = playerCamera.WorldToScreenPoint(medusaEyeTransform.position);

        return medusaPointInScreenSpace.z > 0f
            && medusaPointInScreenSpace.x > xPadding && medusaPointInScreenSpace.x < playerCamera.pixelWidth - xPadding
            && medusaPointInScreenSpace.y > yPadding && medusaPointInScreenSpace.y < playerCamera.pixelHeight - yPadding;
    }
    
    bool IsPlayerInLineOfSight()
    {
        Vector3 medusaGazeDirection = playerEyeTransform.position - medusaEyeTransform.position;

        RaycastHit hit;
        Ray r = new Ray(medusaEyeTransform.position, medusaGazeDirection);
        if (Physics.Raycast(r, out hit, medusaGazeDirection.magnitude) && hit.collider.gameObject.tag == "Player")
        {
            // Player 태그를 가진 EyePosition 객체에 박스 콜라이더 넣어야 Ray가 충돌을 인식함
            // 방패에서 콜라이더를 넣어서 방패로 막으면 Ray를 막도록
            Debug.DrawRay(medusaEyeTransform.position, medusaGazeDirection, Color.red, 0.5f);
            return true;
        }

        return false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Weapon") // Sword
        {
            isAlive = false;
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<WalkInsideCircle>().enabled = false;

            GameObject scoreNotification = Instantiate(score, transform.position, transform.rotation) as GameObject;
            Destroy(scoreNotification, 1.5f);
        }
    }
}
