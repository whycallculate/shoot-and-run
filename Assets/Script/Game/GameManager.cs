using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
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
    [SerializeField] GameObject[] spawnPoint;
    PhotonView pw;
    public int playerindex;


    public bool gameTimeUp = false;

    private void Awake()
    {
    }
    public void Start()
    {
        pw = GetComponent<PhotonView>();
        pw.RPC("AllPlayerisInGame", RpcTarget.AllBuffered);

        StartCoroutine(StartGame());
    }
    public IEnumerator StartGame()
    {
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.MaxPlayers == playerindex)
            {
                if ((int)PhotonNetwork.CurrentRoom.CustomProperties[MatchMakingRoomProperties.GAME_MOD] == 1)
                {
                    DeathMatch.Instance.StartOnDeathMatch();
                    Debug.Log("DUZ OLUM KALIM");
                    break;

                }
                else if ((int)PhotonNetwork.CurrentRoom.CustomProperties[MatchMakingRoomProperties.GAME_MOD] == 2)
                {
                    Debug.Log("TAKIMLI OLUM KALIM");
                    break;
                }
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }

        }
    }
    [PunRPC]
    public void GameEnd()
    {
        if (gameTimeUp)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }


    [PunRPC]
    public void AllPlayerisInGame()
    {
        playerindex++;
    }
    public void PlayerDeath(GameObject player)
    {
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties[MatchMakingRoomProperties.GAME_MOD] == 1)
        {
            StartCoroutine(DeathMatch.Instance.DeathMatchRevive(player));
            Debug.Log("DUZ OLUM KALIM");

        }
        else if((int)PhotonNetwork.CurrentRoom.CustomProperties[MatchMakingRoomProperties.GAME_MOD] == 2)
        {
            Debug.Log("TAKIMLI OLUM KALIM");
        }
        player.SetActive(false);
    }





   
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameTimeUp);
        }
        else
        {
            gameTimeUp = (bool)stream.ReceiveNext();
        }
 
    }
}
