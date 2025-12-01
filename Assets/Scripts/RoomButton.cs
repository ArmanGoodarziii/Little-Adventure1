using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public Text roomName;
    private LobbyManager lobbyManager;

    void Start()
    {
        lobbyManager = FindFirstObjectByType<LobbyManager>();
    }

    public void JoinRoom()
    {
        lobbyManager.JoinRoomInList(roomName.text);
    }
}
