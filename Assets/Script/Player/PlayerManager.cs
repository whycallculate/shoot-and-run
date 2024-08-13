using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
     
    
    [Header("Photon")]
    PhotonView pv;

    [Header("PlayerDataCustomize")]
    [SerializeField] Data myData;
    [SerializeField] PlayerCustomize pmCustom;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (pv.IsMine)
        {
            
            SyncPlayerCustomizeData(PlayerData.Instance);
            pmCustom.SetPlayerBodyColor(myData, PlayerData.Instance.pmData.Body);
        }
    }
    
    #region PlayerCustomize
    public void SyncPlayerCustomizeData(PlayerData pmData)
    {
        string syncString = pmData.PlayerDataToString();
        pv.RPC("RPCSyncPlayerCustomizeData", RpcTarget.AllBuffered, syncString);
        
    }

    void RPCSyncPlayerCustomizeData(string data)
    {
        myData = JsonUtility.FromJson<Data>(data);
        
    }
    #endregion
}
