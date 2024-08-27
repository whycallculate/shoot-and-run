using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerManager>();
            }
            return instance;
        }
    }
    [Header("Photon")]
    PhotonView pw;

    [Header("Player")]
    Vector3 spawnPosition;
    

    private void Awake()
    {
        this.pw = GetComponent<PhotonView>();
        spawnPosition = new Vector3(Random.Range(-21, -100f), transform.position.y, transform.position.z);
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PlayerPrefabs", "Player"), spawnPosition, Quaternion.identity);
        SoundManager.Instance.StopBackgroundMusic();

    }

}
