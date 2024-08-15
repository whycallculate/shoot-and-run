using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    #region singliton
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<PlayerManager>();
            }
            return instance;
        }
    }
    #endregion
    [Header("Photon")]
    PhotonView pv;

    [Header("PlayerDataCustomize")]
    Data myData;
    [SerializeField] PlayerCustomize pmCustom;
    private void Awake()
    {
        pv = LobbyManager.Instance.pw;
    }
    private void Start()
    {
        
        myData = PlayerData.Instance.pmData;

    }

    public void SyncEverytingData()
    {
        
        if (pv.IsMine)
        {
            SyncPlayerCustomizeData(PlayerData.Instance);
            pmCustom.SetPlayerBodyColor(myData, PlayerData.Instance.pmData.Body);
            //pmCustom.SetPlayerNickname(myData, myData.username);
            Debug.Log(PlayerData.Instance.pmData.username);
        }

    }
    
    #region PlayerCustomize
    public void SyncPlayerCustomizeData(PlayerData pmData)
    {
        string syncString = pmData.PlayerDataToString();
        
        myData = JsonUtility.FromJson<Data>(syncString);
        pv.RPC("RPCSyncPlayerCustomizeData", RpcTarget.AllBuffered, syncString);
        
    }

    void RPCSyncPlayerCustomizeData(string data)
    {
        myData = JsonUtility.FromJson<Data>(data);
        
    }
    #endregion
}
