using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class MenuUI : MonoBehaviour
{
    #region singleton
    private static MenuUI instance;
    public static MenuUI Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<MenuUI>();
            }
            return instance;
        }
    }
    #endregion
    [Header("MenuSide")]
    [SerializeField] public GameObject joinSide;
    [SerializeField] public GameObject createSide;
    [SerializeField] public GameObject menuSide;
    [SerializeField] public GameObject roomSide;

    [Header("CreateRoom")]
    [SerializeField] public TMP_InputField roomName;

    [Header("JoinRoomUI")]
    public Transform roomListParent;
    [SerializeField] public GameObject roomItemPrefab;


    [Header("RoomSide")]
    [SerializeField] public Transform playerListParent;
    [SerializeField] public GameObject playerItemList;
    public PhotonView pw;
    private Player player;
    


    [Header("ChatSide")]
    [SerializeField] public GameObject chatSide;

    [Header("MatchMaking")]
    [SerializeField] public GameObject MatchFoundUI;
    public bool acceptOrDecline;
    List<bool> playerBoolCheck = new List<bool>();

    [Header("PlayerCustomize")]
    [SerializeField] private GameObject profilSide;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerCustomize PmCustomize;
    [SerializeField] Color[] defaultColors;
    public int pmParam;    

    

    private void Awake()
    {
        pmParam = 0;
        pw = GetComponent<PhotonView>();
    }


    private void Update()
    {
        ChatSideIsOpen();
        if(PhotonNetwork.CurrentRoom != null )
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        
    }
    #region menuSideAndRoomSide
    public void OpenJoinSide()
    {
        if (createSide.activeSelf == true)
        {
            createSide.SetActive(false);
            joinSide.SetActive(true);
        }
        else if(createSide.activeSelf == false)
        {

            joinSide.SetActive(true);
        } 
    }

    public void OpenCreateSide() 
    {
        if(joinSide.activeSelf == true)
        {
            joinSide.SetActive(false);
            createSide.SetActive(true);
        }
        else if(joinSide.activeSelf == false)
        {

            createSide.SetActive(true);
        }
    }
    public void OpenProfilSide()
    {


        if(profilSide.activeSelf == false)
        {
            profilSide.SetActive(true);
        }
        if (joinSide.activeSelf == true)
        {
            joinSide.SetActive(false);
            createSide.SetActive(false);
        }
        if (createSide.activeSelf == true)
        {
            joinSide.SetActive(false);
            createSide.SetActive(false);
        }
        if (pw.IsMine)
        {
            Instantiate(playerObject);
        }
    }

    public void CreateRoomUISide()
    {
        if(roomName.text.Length == 0)
        {
            Debug.Log("RoomName bos ");
        }
        else if(roomName.text.Length > 0)
        {
            LobbyManager.Instance.CreateRoom(roomName.text);
            createSide.SetActive(false);
            menuSide.SetActive(false);
        }
    }

    public void RoomUpdateUI()
    {
        foreach(Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }
        
        foreach(var room in LobbyManager.Instance.cachedRoom)
        {
            GameObject roomItem = Instantiate(roomItemPrefab, roomListParent);

            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/" + room.MaxPlayers;
            roomItem.transform.GetComponent<JoinButtonByName>().roomName = room.Name;


        }
    }

    public void RoomSideInitiate()
    {
        menuSide.SetActive(false);
        roomSide.SetActive(true);

    }
    
    public void MenuSideInitiate()
    {
        menuSide.SetActive(true);
        roomSide.SetActive(false);
    }
    #endregion
    #region playerlistSide

    public void PlayerAddUI(Player newPlayer)
    {
        Instantiate(playerItemList, playerListParent).GetComponent<PlayerListItem>().PlayerListInitiate(newPlayer);
        Debug.Log(newPlayer.NickName);
        Debug.Log("PlayerUpdateUI");
    }

    public void PlayerUpdateUI(Player[] newPlayer)
    {
        for(int i = 0; i < newPlayer.Length; i++)
        {
            if (newPlayer[i].NickName == playerItemList.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
            {
                Destroy(playerItemList);
            }
            else
            {
                Instantiate(playerItemList, playerListParent).GetComponent<PlayerListItem>().PlayerListInitiate(newPlayer[i]);
            }
            
        }
        
    }

    #endregion
    #region chatSide
    public void ChatSideIsOpen()
    {
        
        if(roomSide.activeSelf == true)
        {
            chatSide.GetComponent<ChatUI>().ChatInputSendMesagge(true);
        }
        else if(roomSide.activeSelf == false)
        {
            chatSide.GetComponent<ChatUI>().ChatInputSendMesagge(false);
        }
    }
    #endregion
    #region MatchMakingUI
    public void AcceptButton()
    {
        pw.RPC("AcceptButtonPunRPCMethod", RpcTarget.All);
    }
    public void DeclineButton() 
    {
        pw.RPC("DeclineButtonPunRPCMethod", RpcTarget.All);
    }
    
    [PunRPC]
    public void AcceptButtonPunRPCMethod()
    {
        acceptOrDecline = true;
        playerBoolCheck.Add(acceptOrDecline);
    }
    [PunRPC]
    public void DeclineButtonPunRPCMethod()
    {
        acceptOrDecline = false;
        playerBoolCheck.Add(acceptOrDecline);
    }
    public void FindMatchMakingButton()
    {

        StartCoroutine(SearchingMatch(1,1,2));
        
    }
    

    IEnumerator SearchingMatch(byte mapType, byte playerLevel , int expectedPlayers)
    {
        int waitForSecond = 1;
        
        while(waitForSecond <100)
        {
            
            if (!MatchMaking.Instance.joinFailed)
            {
                if (PhotonNetwork.CurrentRoom != null)
                {
                    
                    MatchFoundUI.transform.GetChild(0).gameObject.SetActive(false);
                    MatchFoundUI.transform.GetChild(1).gameObject.SetActive(false);

                    if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
                    {
                        MenuSideInitiate();
                        MatchFoundUI.transform.GetChild(1).gameObject.SetActive(true);
                        MatchFoundUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Oda Araniyor : " + waitForSecond.ToString() + " Sn";
                        waitForSecond++;
                        yield return new WaitForSeconds(1);
                    }
                    else if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                    {
                        MatchFoundUI.transform.GetChild(0).gameObject.SetActive(true);
                        MatchFoundUI.transform.GetChild(1).gameObject.SetActive(true);
                        MatchFoundUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Mac Bulundu";
                        if (playerBoolCheck.Count == expectedPlayers)
                        {
                            
                            pw.RPC("MatchFound", RpcTarget.All);

                            break;
                        }



                    }

                }
                else if (PhotonNetwork.CurrentRoom == null)
                {
                    MatchMaking.Instance.CreateRoomForMatchmaking(mapType, playerLevel, expectedPlayers);
                    MatchFoundUI.transform.GetChild(0).gameObject.SetActive(false);
                    MatchFoundUI.transform.GetChild(1).gameObject.SetActive(false);
                }

            }
            else if(MatchMaking.Instance.joinFailed)
            {
                MatchFoundUI.transform.GetChild(0).gameObject.SetActive(false);
                MatchFoundUI.transform.GetChild(1).gameObject.SetActive(false);
                MatchMaking.Instance.joinFailed = false;
                break;
            }
            
            yield return new WaitForSeconds(1);
        }

    }
    [PunRPC]
    public void MatchFound()
    {

        if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (playerBoolCheck.All(x => x == true))
            {
                Debug.Log("Kac Defa calisti");
                //LobbyManager.Instance.PlayerCheckUpdateList();
                RoomSideInitiate();
                
            }
            else if (!playerBoolCheck.All(x => x == false))
            {
                
                playerBoolCheck.Clear();
                MatchFoundUI.transform.GetChild(1).gameObject.SetActive(false);
                MatchFoundUI.transform.GetChild(0).gameObject.SetActive(false);
                MenuSideInitiate();
                PhotonNetwork.LeaveRoom();
                

            }
            else if(playerBoolCheck.All(x => x == false))
            {
                playerBoolCheck.Clear();
                MatchFoundUI.transform.GetChild(1).gameObject.SetActive(false);
                MatchFoundUI.transform.GetChild(0).gameObject.SetActive(false);
                MenuSideInitiate();
                PhotonNetwork.LeaveRoom();
            }

        }


    }
    #endregion
    #region PlayerCustomize
    public void ChangeColorOnBody(int param)
    {

        PlayerData.Instance.pmData.Body += param;
        Debug.Log(PlayerData.Instance.pmData.Body);
        if (PlayerData.Instance.pmData.Body > defaultColors.Length)
        {

            MenuUI.Instance.pmParam = 0;
            PlayerData.Instance.pmData.Body = 0;
        }
        if (PlayerData.Instance.pmData.Body < 0)
        {
            PlayerData.Instance.pmData.Body = defaultColors.Length - 1;
        }
        PlayerPrefs.SetInt("Body", PlayerData.Instance.pmData.Body);
        int i = PlayerData.Instance.pmData.Body;
        PmCustomize.SetPlayerBodyColor(PlayerData.Instance.pmData, i);
    }
    

    #endregion
}
