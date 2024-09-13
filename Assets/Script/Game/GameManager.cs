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
    int playerindex;

    int Minute = 9;
    int second = 60;
    int totalTime = 600;
    bool gameTimeUp = false;


    public void Start()
    {
        pw= GetComponent<PhotonView>();
        PhotonNetwork.ConnectUsingSettings();
        //spawnPosition = new Vector3(Random.Range(-21, -100f), transform.position.y, transform.position.z);
        //GameObject player = PhotonNetwork.Instantiate(Path.Combine("PlayerPrefabs", "Player"), spawnPosition, Quaternion.identity);
        //SoundManager.Instance.StopBackgroundMusic();
        //if (player.transform.GetChild(1).GetComponent<PhotonView>().IsMine)
        //{
        //    GameUI.Instance.GetPlayerData(player);
        //}
        
    }
    private void Update()
    {
        Debug.LogError(gameTimeUp);
        Debug.Log(playerindex);
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("test", roomOptions,null);
        
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.NickName = "test" + PhotonNetwork.LocalPlayer.ActorNumber;
        Vector3 pos = spawnPoint[Random.Range(0, 9)].transform.position;
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PlayerPrefabs", "Player"), pos, Quaternion.identity);
        player.transform.GetChild(1).position = pos;
        SoundManager.Instance.StopBackgroundMusic();
        GameDM();
        if (player.transform.GetChild(1).GetComponent<PhotonView>().IsMine)
        {
            GameUI.Instance.GetPlayerData(player);
        }
        pw.RPC("GetTime", RpcTarget.AllBuffered,Minute,second,totalTime);
        StartCoroutine(GameDM());


    }



    [PunRPC]
    public void GetTime(int minute,int second,int totalTime)
    {
        playerindex++;
        Minute = minute;
        this.second = second;
        this.totalTime = totalTime;
    }



    public IEnumerator DeathMatchRevive(GameObject player)
    {
        Vector3 pos = spawnPoint[Random.Range(0, 9)].transform.position;
        yield return new WaitForSeconds(3f);
        player.transform.position = pos;
        player.SetActive(true);


    }
    public void PlayerDeath(GameObject player)
    {
        player.SetActive(false);
        StartCoroutine(DeathMatchRevive(player));
    }
    public IEnumerator GameDM()
    {
        while(true)
        {
            if(PhotonNetwork.CurrentRoom.MaxPlayers != playerindex)
            {
                GameUI.Instance.GameWaitingPlayers();
                yield return new WaitForSeconds(1f);
            }
            else if(PhotonNetwork.CurrentRoom.MaxPlayers == playerindex)
            {
                GameUI.Instance.GameOnUI();
                StartCoroutine(StartTimer());
                break;
            }
        }
    }
    public IEnumerator StartTimer()
    {

        while (true)
        {
            if (totalTime > 0)
            {
                if (second > 0)
                {
                    second--;
                    totalTime--;
                    yield return new WaitForSeconds(1f);
                }

                else if (second <= 0)
                {

                    Minute--;
                    second = 60;
                    second--;
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                gameTimeUp = true;
                break;
            }
            GameUI.Instance.gameTimer.text = string.Format("{0:00}:{1:00}", Minute, second);
        }

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerindex);
        }
 
    }
}
