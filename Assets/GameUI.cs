using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{

    private static GameUI instance;
    public static GameUI Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GameUI>();
            }
            return instance;
        }
    }

    [Header("PlayerInfo")]
    public GameObject player;
    public TextMeshProUGUI pmSpeed;
    public TextMeshProUGUI pmHealth;
    public TextMeshProUGUI pmArmor;
    public GameObject pmAmmo;



    private void Update()
    {
        if(player != null)
        {
            pmSpeed.text = player.transform.GetChild(1).GetComponent<Rigidbody>().velocity.magnitude.ToString();

        }
    }
    public void GetPlayerData(GameObject player)
    {
        this.player = player;
    }
}
