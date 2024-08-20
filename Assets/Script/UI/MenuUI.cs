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
    [SerializeField] public GameObject SceneAnimImage;

    [Header("RoomSide")]
    [SerializeField] public Transform playerListParentLeft;
    [SerializeField] public Transform playerListParentRight;
    [SerializeField] public GameObject playerItemList;
    public PhotonView pw;

    [Header("SettingsSide")]
    [SerializeField] private GameObject settingsMenu;
    [Header("ChatSide")]
    [SerializeField] public GameObject chatSide;

    [Header("Console")]
    [SerializeField] private GameObject console;

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
    private void Start()
    {
        SceneAnimImage.GetComponent<UIAnim>().NewSceneChange(SceneAnimImage);
    }

    private void Update()
    {
        //Chat side acik mi kapali mi kismini updatte calistiriyoruz 
        ChatSideIsOpen();
        OpenAndCloseConsole();
        if (PhotonNetwork.CurrentRoom != null )
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        
    }
    #region menuSideAndRoomSide


    public void OpenAndCloseConsole()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (console.activeSelf)
            {
                console.SetActive(false);
            }
            else
            {
                 console.SetActive(true);
            }
        }
    }
    public void OpenProfilSide()
    {
        //Profili acilip kapandigi kisim

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
        // oda yukleme ekrani
        menuSide.SetActive(false);
        roomSide.SetActive(true);
        SoundManager.Instance.StopBackgroundMusic();
    }
    public void OpenAndCloseSettingsMenu()
    {
        if(settingsMenu.activeSelf) 
        {
            settingsMenu.GetComponent<UIAnim>().OnDisabled();
        }
        else if (!settingsMenu.activeSelf)
        {
            settingsMenu.GetComponent<UIAnim>().OnEnabled();
        }
        if (profilSide.activeSelf)
        {
            settingsMenu.GetComponent<UIAnim>().OnDisabled();
        }
        
    }
    
    public void MenuSideInitiate()
    {
        //Menu Yukleme ekrani
        menuSide.SetActive(true);
        roomSide.SetActive(false);
        
    }
    #endregion
    #region chatSide
    public void ChatSideIsOpen()
    {
        //Room Aktif oldugu zaman chatin calisicagi kisim.
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
        //Oyunculararin Accept butonuna bastigi callback.

        pw.RPC("AcceptButtonPunRPCMethod", RpcTarget.All);
    }
    public void DeclineButton() 
    {
        //Oyunculararin Accept butonuna bastigi callback.

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
        // Eger oyuncunun ismi belirli bir aralikta ise Arama yapilacak kontrol kismi.
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
        //Aramayi Iptal etmek icin 
        cancelMatchMaking = true;
        Debug.Log(cancelMatchMaking);
    }


    IEnumerator SearchingMatch(byte mapType, byte playerLevel, int expectedPlayers)
    {
        int waitForSecond = 1;
        
        //100 Saniye boyunca arama yaparak oda bulmaya calistigi kisim waitforSecond degistirerek Timeout suresini degistirebilirsin.
        while (waitForSecond < 100)
        {
            //Eger iptal butonuna basilmadiysa
            if (!cancelMatchMaking)
            {
                //eger birisi quitlemdiyse
                if (!MatchMaking.Instance.joinFailed)
                {
                    //Eger suan da bir odadaysak Yapilacak islemler
                    if (PhotonNetwork.CurrentRoom != null)
                    {


                        //Yeterli oyuncu gelene kadar Bekledigimiz kisim Oyuncu arama kismi
                        if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
                        {
                            MenuSideInitiate();
                            MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnEnabled();
                            MatchFoundUI.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = waitForSecond.ToString() + " Sn";
                            waitForSecond++;
                            yield return new WaitForSeconds(0.1f);
                        }
                        // Eger yeterli oyuncuya ulasildiysa Artik mac bulundu yapilip oyuncularin kabul kontrolleri yapiliyor.
                        else if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                        {
                            MatchFoundUI.transform.GetChild(0).GetComponent<UIAnim>().OnEnabled();
                            MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnEnabled();
                            MatchFoundUI.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Mac Bulundu";
                            //Eger herkes accept veya decline bastiysa Gerekli kontrollerin olacagi RPC methodu
                            if (playerBoolCheck.Count == expectedPlayers)
                            {

                                pw.RPC("MatchFound", RpcTarget.All);

                                SoundManager.PlaySoundRandom(SoundType.UI_NEXT, 1);
                                break;
                            }



                        }

                    }
                    else if (PhotonNetwork.CurrentRoom == null)
                    {
                        MatchMaking.Instance.CreateRoomForMatchmaking(mapType, playerLevel, expectedPlayers);
                        MatchFoundUI.transform.GetChild(0).GetComponent<UIAnim>().OnDisabled();
                        MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnDisabled();
                        playerBoolCheck.Clear();
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
        //Gene oda eger full ise yapilacak islemler ufak bir bug kontrolu.
        if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            //Oyuncularin Hepsinin bool true dondurdugune bakiliyor.
            if (playerBoolCheck.All(x => x == true))
            {
                Debug.Log("Kac Defa calisti");
                SceneAnimImage.GetComponent<UIAnim>().ChangeImageRoom(SceneAnimImage);
                
                
            }
            //Decline basan varmi.
            else if (!playerBoolCheck.All(x => x == false))
            {
               
                playerBoolCheck.Clear();
                MatchFoundUI.transform.GetChild(0).GetComponent<UIAnim>().OnDisabled();
                MatchFoundUI.transform.GetChild(1).GetComponent<UIAnim>().OnDisabled();
                MenuSideInitiate();
                PhotonNetwork.LeaveRoom();

                
            }
            //Decline basan varmi.
            else if (playerBoolCheck.All(x => x == false))
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
