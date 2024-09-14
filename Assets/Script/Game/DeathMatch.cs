using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class DeathMatch : MonoBehaviour
{
    private static DeathMatch instance;
    public static DeathMatch Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DeathMatch>();
            }
            return instance;
        }
    }
    [SerializeField] GameObject[] spawnPoint;

    int Minute = 9;
    int second = 60;
    int totalTime = 600;
    PhotonView pw;
    public void StartOnDeathMatch()
    {
        pw = GetComponent<PhotonView>();

        Vector3 pos = spawnPoint[Random.Range(0, 9)].transform.position;
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PlayerPrefabs", "Player"), pos, Quaternion.identity);
        player.transform.GetChild(1).position = pos;

        SoundManager.Instance.StopBackgroundMusic();

        if (player.transform.GetChild(1).GetComponent<PhotonView>().IsMine)
        {
            GameUI.Instance.GetPlayerData(player);
        }

        pw.RPC("GetTime", RpcTarget.AllBuffered, Minute, second, totalTime);
        StartCoroutine(StartOnGame());
    }

    public IEnumerator DeathMatchRevive(GameObject player)
    {
        Vector3 pos = spawnPoint[Random.Range(0, 9)].transform.position;
        yield return new WaitForSeconds(3f);
        player.transform.position = pos;
        player.SetActive(true);


    }
    public void PlayerDeathDeathMatch(GameObject player)
    {
        player.SetActive(false);
        StartCoroutine(DeathMatchRevive(player));
    }
    public IEnumerator StartOnGame()
    {
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.MaxPlayers != GameManager.Instance.playerindex)
            {
                GameUI.Instance.GameWaitingPlayers();
                yield return new WaitForSeconds(1f);
            }
            else if (PhotonNetwork.CurrentRoom.MaxPlayers == GameManager.Instance.playerindex)
            {
                GameUI.Instance.GameOnUI();
                StartCoroutine(StartTimer());
                break;
            }
        }
    }

    [PunRPC]
    public void GetTime(int minute, int second, int totalTime)
    {
        Minute = minute;
        this.second = second;
        this.totalTime = totalTime;
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
                GameManager.Instance.gameTimeUp = true;
                yield return new WaitForSeconds(3f);
                pw.RPC("GameEndDeathMatch", RpcTarget.AllBuffered);
                break;
            }
            GameUI.Instance.gameTimer.text = string.Format("{0:00}:{1:00}", Minute, second);
        }

    }
    public void GameEndDeathMatch()
    {
        GameManager.Instance.GameEnd();
    }
}
