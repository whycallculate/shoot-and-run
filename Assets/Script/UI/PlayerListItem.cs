using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    private static PlayerListItem instance;
    public static PlayerListItem Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<PlayerListItem>();
            }
            return instance;
        }
    }

    [Header("Playerlist")]
    [SerializeField] private Transform contentLeft;
    [SerializeField] private Transform contentRight;
    [SerializeField] private GameObject playerItemPrefab;
    [SerializeField] private GameObject RoomUI;
    
    //public List<Player> playerList = new List<Player>();
    private PhotonView pw; 
    private void Awake()
    {
        pw = GetComponent<PhotonView>();
    }
    private void Update()
    {
        int i = contentLeft.childCount + contentRight.childCount;
        if (i > contentLeft.childCount + contentRight.childCount)
        {
            i = 0;
        }
        

        if (RoomUI.activeSelf)
        {

            if (i != PhotonNetwork.CurrentRoom.PlayerCount)
            {

                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    Debug.Log(player.NickName);
                    AddPlayerToList(player);


                }
            }
            

            BalancePlayerLists();
        }
    }



    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerFromList(otherPlayer);
        BalancePlayerLists();
    }


    public void AddPlayerToList(Player player)
    {


        Transform targetContent = contentLeft.childCount <= contentRight.childCount ? contentLeft : contentRight;
        GameObject newPlayerName = Instantiate(playerItemPrefab, targetContent);
        newPlayerName.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.NickName;
        newPlayerName.name = "Player_" + player.ActorNumber;
        if(targetContent == contentLeft)
        {
            GetPlayerTeam(PhotonNetwork.LocalPlayer, Team.Blue);
        }
        else if(targetContent == contentRight)
        {
            GetPlayerTeam(PhotonNetwork.LocalPlayer, Team.Red);

        }


    }
    
    public void GetPlayerTeam(Player player,Team team)
    {
        PlayerData.Instance.playerTeams[player] = team;
    }
    private void RemovePlayerFromList(Player player)
    {
        //Cok saglikli degil ama Find ile Content gameobjectlerin icini gezip varmi yokmu diye bakiyor.
        Transform playerNameLeft = contentLeft.Find("Player_" + player.ActorNumber);
        Transform playerNameRight = contentRight.Find("Player_" + player.ActorNumber);

        if (playerNameLeft != null)
        {
            Destroy(playerNameLeft.gameObject);
        }
        else if (playerNameRight != null)
        {
            Destroy(playerNameRight.gameObject);
        }
    }

    private void BalancePlayerLists()
    {
        // Sol ve sağ taraftaki çocukların sayısını dengele
        while (contentLeft.childCount > contentRight.childCount + 1)
        {
            Transform lastChild = contentLeft.GetChild(contentLeft.childCount - 1);
            lastChild.SetParent(contentRight);
        }

        while (contentRight.childCount > contentLeft.childCount + 1)
        {
            Transform lastChild = contentRight.GetChild(contentRight.childCount - 1);
            lastChild.SetParent(contentLeft);
        }
    }
    public void ClearPlayerList()
    {
        foreach (Transform child in contentLeft)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in contentRight)
        {
            Destroy(child.gameObject);
        }
    }
}
