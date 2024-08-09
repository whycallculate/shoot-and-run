using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinButtonByName : MonoBehaviour
{
    public string roomName;

    public void JoinButtonPressed()
    {
        LobbyManager.Instance.JoinRoom(roomName);
    }
}
