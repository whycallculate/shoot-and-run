using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    Player player;

    public void PlayerListInitiate(Player pm)
    {
        player = pm;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = pm.NickName;
        for (int i =0;i <PhotonNetwork.PlayerListOthers.Length; i++) 
        {
            if(PhotonNetwork.PlayerListOthers[i].NickName != pm.NickName)
            {
                Destroy(gameObject);
            }
            
        }

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }

        
    }
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
