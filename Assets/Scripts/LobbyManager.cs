using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField inputField;
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(inputField.text, new RoomOptions(){MaxPlayers = 4 , IsVisible = true, IsOpen = true}, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Level");
    }
}
