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
				PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, 0f, randomPoint), Quaternion.identity); // 프리펩이 Resources폴더에 있어야함
			}
            else
            {
                Debug.Log("Place playerPrefab!");
            }
        }
    }
}
