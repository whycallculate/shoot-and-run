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

    public PhotonView pw;
    public Player Player;
    private void Start()
    {
        pw = GetComponent<PhotonView>();
        
    }
    public override void OnLeftRoom()
    {
        MenuUI.Instance.MenuSideInitiate();
        
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        StartCoroutine(MainWeb.Instance.web.Logout(PhotonNetwork.NickName));
        SoundManager.Instance.StopBackgroundMusic();
    }
    private void OnApplicationQuit()
    {
        SoundManager.Instance.StopBackgroundMusic();

        StartCoroutine(MainWeb.Instance.web.Logout(PhotonNetwork.NickName));
    }


    
}
