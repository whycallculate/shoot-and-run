using Photon.Pun;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    [Header("Player")]
    Vector3 spawnPosition;
    public void Start()
    {

        PhotonNetwork.ConnectUsingSettings();
        //spawnPosition = new Vector3(Random.Range(-21, -100f), transform.position.y, transform.position.z);
        //GameObject player = PhotonNetwork.Instantiate(Path.Combine("PlayerPrefabs", "Player"), spawnPosition, Quaternion.identity);
        //SoundManager.Instance.StopBackgroundMusic();
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
        spawnPosition = new Vector3(Random.Range(-21, -100f), transform.position.y, transform.position.z);
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PlayerPrefabs", "Player"), spawnPosition, Quaternion.identity);
        SoundManager.Instance.StopBackgroundMusic();
    }



}
