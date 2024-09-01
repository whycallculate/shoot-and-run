using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject PlayerManager;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
    public override void OnJoinedRoom()
    {
        PlayerManager.SetActive(true);
    }
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.C))
        {
           //AimState.Instance.rig.Clear();
           //AimState.Instance.rig.Build();
           //AimState.Instance.anim.Rebind();
           //AimState.Instance.anim.Update(0f);


        }

    }
}
