using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static LobbyManager instance;
    public static LobbyManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<LobbyManager>();
            }
            return instance;
        }
    }
    #endregion

    public List<RoomInfo> cachedRoom = new List<RoomInfo>();

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("OnConnectedToMaster");
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.NickName = "Player" + Random.Range(0, 100).ToString();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(cachedRoom.Count <= 0) 
        {
            cachedRoom = roomList;
        }
        else
        {
            foreach(var room in roomList)
            {
                for(int i = 0; i < cachedRoom.Count; i++)
                {
                    if (cachedRoom[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cachedRoom;

                        if (room.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i] = room;
                        }
                        cachedRoom = newList;
                    }
                }
            }
        }

        MenuUI.Instance.RoomUpdateUI();
    }

    public void MatchMaking()
    {
   
    }


    public void CreateRoom(string roomName)
    {
        Debug.Log("CreateRoom");
        PhotonNetwork.CreateRoom(roomName);
        MenuUI.Instance.RoomSideInitiate();
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        MenuUI.Instance.RoomSideInitiate();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        MenuUI.Instance.PlayerAddUI(newPlayer);
        

    }
    public override void OnJoinedRoom()
    {

        PlayerCheckUpdateList();
    }
    public override void OnLeftRoom()
    {
        MenuUI.Instance.MenuSideInitiate();
        
    }


    public void PlayerCheckUpdateList()
    {
        Player[] players = PhotonNetwork.PlayerList;
        MenuUI.Instance.PlayerUpdateUI(players);
    }
}
