using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedObjController : MonoBehaviour
{
    PlayerManager playerManager;

    [SerializeField] Transform sphere;

    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();

        sphere = transform.GetChild(0); // 이 스크립트가 부착되어있는 오브젝트의 자식개체중 첫번째것

        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
            
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            GetComponent<Renderer>().material = playerManager.collectedObjMat;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // CollectibleObj를 콜렉트하기위함
        if (other.gameObject.CompareTag("CollectibleObj"))
        {
            if (!playerManager.collidedList.Contains(other.gameObject))
            {
                other.gameObject.tag = "CollectedObj";
                other.transform.parent = playerManager.collectedPoolTransform;
                playerManager.collidedList.Add(other.gameObject);
                other.gameObject.AddComponent<CollectedObjController>();
            }
        }

        if (other.gameObject.CompareTag("Obstacle"))
        {
            DestroyTheObject();
        }
    }

    void DestroyTheObject()
    {
        playerManager.collidedList.Remove(gameObject);
        Destroy(gameObject);

        Transform particle = Instantiate(playerManager.particlePrefab, transform.position, Quaternion.identity);
        particle.GetComponent<ParticleSystem>().startColor = playerManager.collectedObjMat.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        // CollectibleList를 콜렉트 하기위함
        if (other.gameObject.CompareTag("CollectibleList"))
        {
            other.transform.GetComponent<BoxCollider>().enabled = false;
            other.transform.parent = playerManager.collectedPoolTransform;

            foreach (Transform child in other.transform)
            {
                if (!playerManager.collidedList.Contains(child.gameObject))
                {
                    playerManager.collidedList.Add(child.gameObject);
                    child.gameObject.tag = "CollectedObj";
                    child.gameObject.AddComponent<CollectedObjController>();
                }
            }
        }

        if (other.gameObject.CompareTag("FinishLine"))
        {
            if (playerManager.levelState != PlayerManager.LevelState.Finished)
            {
                playerManager.levelState = PlayerManager.LevelState.Finished;
                playerManager.CallMakeSphere();
            }
        }
    }

    public void MakeShpere()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        sphere.gameObject.GetComponent<MeshRenderer>().enabled = true;
        sphere.gameObject.GetComponent<SphereCollider>().enabled = true;
        sphere.gameObject.GetComponent<SphereCollider>().isTrigger = true;
        sphere.gameObject.GetComponent<Renderer>().material = playerManager.collectedObjMat;
    }

    public void DropObj()
    {
        sphere.gameObject.layer = 8; // FinishGroundLayer
        sphere.gameObject.GetComponent<SphereCollider>().isTrigger = false;
        sphere.gameObject.AddComponent<Rigidbody>();
        sphere.GetComponent<Rigidbody>().useGravity = true;
    }
}
