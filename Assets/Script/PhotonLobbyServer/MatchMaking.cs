using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MatchMaking : MonoBehaviour, IMatchmakingCallbacks
{
    #region singletion
    private static MatchMaking instance;
    public static MatchMaking Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MatchMaking>();
            }
            return instance;
        }
    }
    #endregion

    public const string MAP_KEY = "map";
    public const string GAME_MOD = "gm";
    public const string PLAYER_LEVEL = "lvl";
    public bool joinFailed = false;

    private LoadBalancingClient loadBalancingClient;
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void CreateRoomForMatchmaking(byte mapKey, byte playerLevel, int expectedPlayers)
    {
        Hashtable expectedCustomRoomProp = new Hashtable { { MAP_KEY, mapKey }, { PLAYER_LEVEL, playerLevel } };
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = expectedCustomRoomProp;
        roomOptions.MaxPlayers = expectedPlayers;
        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomOptions = roomOptions;
        Debug.Log("CreateRoomForMatchmaking");
        //loadBalancingClient = new LoadBalancingClient();
        //loadBalancingClient.OpCreateRoom(enterRoomParams);
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions);
        

    }
    

    
    public void JoinRandonMatchmaking(byte mapKey, byte playerLevel)
    {
        Hashtable expectedCustomRoomProp = new Hashtable { { MAP_KEY, mapKey }, { PLAYER_LEVEL, playerLevel } };
        OpJoinRandomRoomParams opJoinRandomRoomParams = new OpJoinRandomRoomParams();
        opJoinRandomRoomParams.ExpectedMaxPlayers = 2;
        opJoinRandomRoomParams.ExpectedCustomRoomProperties = expectedCustomRoomProp;
        loadBalancingClient.OpJoinRandomRoom(opJoinRandomRoomParams);

    }

    #region IMatchmakingCallbacks
    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        throw new System.NotImplementedException();
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnJoinedRoom()
    {
        throw new System.NotImplementedException();
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        joinFailed = true;
    }

    public void OnLeftRoom()
    {
        throw new System.NotImplementedException();
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
