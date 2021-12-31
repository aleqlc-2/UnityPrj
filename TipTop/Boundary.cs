using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other) // Exit
    {
        if (other.CompareTag("Obstacle"))
        {
            other.gameObject.SetActive(false);
            ObstacleGenerator.Obstacles.Enqueue(other.gameObject);
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
