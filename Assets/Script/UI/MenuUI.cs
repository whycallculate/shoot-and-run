using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private GameObject joinSide;
    [SerializeField] private GameObject createSide;

    [Header("CreateRoom")]
    [SerializeField] public TMP_InputField roomName;

    [Header("JoinRoomUI")]
    public Transform roomListParent;
    [SerializeField] public GameObject roomItemPrefab;

    private void Update()
    {
        
    }
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

    public void CreateRoomUISide()
    {
        if(roomName.text.Length == 0)
        {
            Debug.Log("RoomName bos ");
        }
        else if(roomName.text.Length > 0)
        {
            LobbyManager.Instance.CreateRoom(roomName.text);
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


        }
    }

}
