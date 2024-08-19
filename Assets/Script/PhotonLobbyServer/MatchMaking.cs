using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    public const string MAP_KEY = "map";
    public const string GAME_MOD = "gm";
    public const string PLAYER_LEVEL = "lvl";
    public bool joinFailed = false;
    Player player;
    Player otherPlayer;



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
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions);
        

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
