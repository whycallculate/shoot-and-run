using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    private static ChatUI instance;
    public static ChatUI Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<ChatUI>();
            }
            return instance;
        }
    }
    [Header("Chat")]
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private GameObject messageItem;
    [SerializeField] private Transform messageParent;
    private PhotonView pw;


    private void Awake()
    {
        pw = GetComponent<PhotonView>();
    }
    public void ChatInputSendMesagge(bool isOpen)
    {
        string text = PhotonNetwork.NickName + " : " + chatInput.text;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            pw.RPC("GetMessage", RpcTarget.All, text, isOpen);
        }
        
        
    }
    public void ClearMessage()
    {
        foreach (Transform child in messageParent)
        {
            Destroy(child.gameObject);
        }
    }

    [PunRPC]
    public void GetMessage(string text,bool isOpen)
    {
        
        if (isOpen)
        {
            messageItem.GetComponent<TextMeshProUGUI>().text = text;

            Instantiate(messageItem, messageParent);
        }
        else
        {
            Destroy(messageItem);
        }
        

    } 


}
