using Photon.Pun;
using UnityEngine;

public class MobileFPSGameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (playerPrefab != null)
            {
				int randomPoint = Random.Range(-10, 10);
				PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, 0f, randomPoint), Quaternion.identity); // �������� Resources������ �־����
			}
            else
            {
                Debug.Log("Place playerPrefab!");
            }
        }
    }
}
