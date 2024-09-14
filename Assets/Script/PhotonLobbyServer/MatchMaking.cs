using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public static class MatchMakingRoomProperties
{
    public static string MAP_KEY = "map";
    public static string GAME_MOD = "gm";
    public static string PLAYER_LEVEL = "lvl";
}
public class MatchMaking : MonoBehaviourPunCallbacks
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


    public bool joinFailed = false;
    Player player;
    Player otherPlayer;

    byte mapKey;
    int gameMode;
    int expectedPlayers;

    private LoadBalancingClient loadBalancingClient;
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void CreateRoomForMatchmaking(byte mapKey, int gameMode, int expectedPlayers)
    {
        Hashtable expectedCustomRoomProp = new Hashtable { { MatchMakingRoomProperties.MAP_KEY, mapKey }, { MatchMakingRoomProperties.GAME_MOD, gameMode } };
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby =  new string[] { MatchMakingRoomProperties.MAP_KEY,MatchMakingRoomProperties.GAME_MOD};
        roomOptions.CustomRoomProperties = expectedCustomRoomProp;
        roomOptions.MaxPlayers = expectedPlayers;
        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomOptions = roomOptions;
        PhotonNetwork.CreateRoom(null,roomOptions: roomOptions,null);
        

    }
    public void JoinRandomRoomMatchMaking(byte mapKey, int gameMode, int expectedPlayers)
    {
        this.mapKey = mapKey;
        this.gameMode = gameMode;
        this.expectedPlayers = expectedPlayers;
        Hashtable expectedCustomRoomProp = new Hashtable { { MatchMakingRoomProperties.MAP_KEY, mapKey }, { MatchMakingRoomProperties.GAME_MOD, gameMode } };
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = expectedCustomRoomProp;
        roomOptions.MaxPlayers = expectedPlayers;
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProp,expectedPlayers);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoomForMatchmaking(mapKey, gameMode, expectedPlayers);
    }

    #region IMatchmakingCallbacks

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
        if (PhotonNetwork.LocalPlayer != otherPlayer)
        {
            PhotonNetwork.LeaveRoom();

            MenuUI.Instance.MatchFoundUI.transform.GetChild(0).GetComponent<UIAnim>().OnDisabled();
            MenuUI.Instance.MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnDisabled();
        }
        MenuUI.Instance.SceneAnimImage.GetComponent<UIAnim>().ChangeImageMenu(MenuUI.Instance.SceneAnimImage);

        joinFailed = true;
    }

    

    #endregion
}
