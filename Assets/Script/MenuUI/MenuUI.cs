using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


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
    [SerializeField] public GameObject menuSide;
    [SerializeField] public GameObject roomSide;

    [Header("RoomSide")]
    [SerializeField] public Transform playerListParent;
    [SerializeField] public GameObject playerItemList;
    public PhotonView pw;


    [Header("ChatSide")]
    [SerializeField] public GameObject chatSide;

    [Header("MatchMaking")]
    [SerializeField] public GameObject MatchFoundUI;
    public bool acceptOrDecline;
    public bool cancelMatchMaking = false;
    List<bool> playerBoolCheck = new List<bool>();

    [Header("PlayerCustomizeAndPlayerStuff")]
    [SerializeField] private GameObject profilSide;
    [SerializeField] public GameObject playerObject;
    [SerializeField] private PlayerCustomize PmCustomize;
    [SerializeField] Color[] defaultColors;
    public int pmParam;
    [SerializeField] private TMP_InputField nickNameInputText;
    

    

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



    public void OpenProfilSide()
    {


        if (profilSide.activeSelf == false)
        {
            profilSide.SetActive(true);
        }
        else if (profilSide.activeSelf == true)
        {
            profilSide.SetActive(false);
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

        for(int i = 0;i < PhotonNetwork.PlayerList.Length;i++)
        {
            if(playerItemList.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == PhotonNetwork.PlayerList[i].NickName)
            {
                Instantiate(playerItemList, playerListParent).GetComponent<PlayerListItem>().PlayerListInitiate(newPlayer);
            }
            else
            {

            }
        }
      
        
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
        if (PhotonNetwork.NickName.Length >= 4 && PhotonNetwork.NickName.Length <= 14)
        {
            StartCoroutine(SearchingMatch(1, 1, 2));
        }
        else
        {
            Debug.Log("set you nickname");
        }
        
        
    }
    public void CancelMatchMakingButton()
    {
        cancelMatchMaking = true;
        Debug.Log(cancelMatchMaking);
    }


    IEnumerator SearchingMatch(byte mapType, byte playerLevel, int expectedPlayers)
    {
        int waitForSecond = 1;

        while (waitForSecond < 100)
        {

            if (!cancelMatchMaking)
            {
                if (!MatchMaking.Instance.joinFailed)
                {
                    if (PhotonNetwork.CurrentRoom != null)
                    {

                        //MatchFoundUI.transform.GetChild(0).gameObject.SetActive(false);
                        //MatchFoundUI.transform.GetChild(1).gameObject.SetActive(false);

                        if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
                        {
                            MenuSideInitiate();
                            MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnEnabled();
                            MatchFoundUI.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = waitForSecond.ToString() + " Sn";
                            waitForSecond++;
                            yield return new WaitForSeconds(0.1f);
                        }
                        else if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                        {
                            MatchFoundUI.transform.GetChild(0).GetComponent<UIAnim>().OnEnabled();
                            MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnEnabled();
                            MatchFoundUI.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Mac Bulundu";
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
                        MatchFoundUI.transform.GetChild(0).GetComponent<UIAnim>().OnDisabled();
                        MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnDisabled();
                    }

                }
                else if (MatchMaking.Instance.joinFailed)
                {
                    MatchFoundUI.transform.GetChild(0).GetComponent<UIAnim>().OnDisabled();
                    MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnDisabled();
                    MatchMaking.Instance.joinFailed = false;
                    break;
                }
            }
            else if (cancelMatchMaking) 
            {
                MatchFoundUI.transform.GetChild(0).GetComponent<UIAnim>().OnDisabled();
                MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnDisabled();
                PhotonNetwork.LeaveRoom();
                cancelMatchMaking = false;
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
            //Oyuncularin Hepsinin bool true dondurdugune bakiliyor.
            if (playerBoolCheck.All(x => x == true))
            {
                Debug.Log("Kac Defa calisti");
                //LobbyManager.Instance.PlayerCheckUpdateList();
                RoomSideInitiate();
                
            }
            else if (!playerBoolCheck.All(x => x == false))
            {
                
                playerBoolCheck.Clear();
                MatchFoundUI.transform.GetChild(0).GetComponent<UIAnim>().OnDisabled();
                MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnDisabled();
                MenuSideInitiate();
                PhotonNetwork.LeaveRoom();
                

            }
            else if(playerBoolCheck.All(x => x == false))
            {
                playerBoolCheck.Clear();
                MatchFoundUI.transform.GetChild(0).GetComponent<UIAnim>().OnDisabled();
                MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnDisabled();
                MenuSideInitiate();
                PhotonNetwork.LeaveRoom();
            }


            

        }


    }
    #endregion
    #region PlayerCustomizeAndPlayerStuff
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
        PlayerPrefs.Save();

    }
    

    #endregion
}
