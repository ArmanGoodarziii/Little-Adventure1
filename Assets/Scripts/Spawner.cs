using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    void Start()
    {
        PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(Random.Range(-5,-3), 0.5f, 0), Quaternion.identity);
    }
}
