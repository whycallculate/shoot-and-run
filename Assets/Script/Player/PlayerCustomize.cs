using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomize : MonoBehaviour
{
    
    
    [SerializeField] Color[] bodycolor;
    [SerializeField] public Material bodyMat;

    private void Start()
    {
        
    }
    public void SetPlayerBodyColor(Data pmData , int colorIndex)
    {
        bodyMat.color = bodycolor[colorIndex];
        pmData.Body = colorIndex;
        
        
    }
    public void SetPlayerNickname(Data pmData,string nickName)
    {

        PhotonNetwork.NickName = nickName;
        pmData.nickName = nickName;
    }

}
