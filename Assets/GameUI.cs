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
    public GameObject pmAmmoMain;
    public GameObject pmAmmoSecondary;

    [Header("Canvas")]
    public GameObject waitingPlayer;
    public GameObject gameElements;
    public GameObject timer;
    public GameObject crosshair;

    public TextMeshProUGUI gameTimer;

    private void Update()
    {
        if(player != null)
        {

            
            GetPlayerDataOnUI();
        }
    }

    public void GameWaitingPlayers()
    {
        waitingPlayer.SetActive(true);
        gameElements.SetActive(false);
        timer.SetActive(false);
        crosshair.SetActive(false);

    }
    public void GameOnUI()
    {
        waitingPlayer.SetActive(false);
        gameElements.SetActive(true);
        timer.SetActive(true);
        crosshair.SetActive(true);
    }
    private void GetPlayerDataOnUI()
    {
        int i = 0;
        int lastIndex;
        pmSpeed.text = player.transform.GetChild(1).GetComponent<Rigidbody>().velocity.magnitude.ToString();
        pmHealth.text = player.transform.GetChild(1).GetComponent<PlayerManager>().currentHP.ToString();
        pmArmor.text = player.transform.GetChild(1).GetComponent<PlayerManager>().currentArmor.ToString();
        if (player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainIsActive)
        {
            if(player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.type == WeaponType.RIFLE) 
            {
                lastIndex = i;
                pmAmmoMain.transform.GetChild(lastIndex).gameObject.SetActive(false);
                i = 2;
                pmAmmoMain.transform.GetChild(i).gameObject.SetActive(true);
                pmAmmoMain.GetComponent<TextMeshProUGUI>().text = player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.weapon.currentAmmo.ToString() + " / " + player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.weapon.totalAmmo.ToString();
            }
            else if(player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.type == WeaponType.SMG)
            {
                lastIndex = i;
                pmAmmoMain.transform.GetChild(lastIndex).gameObject.SetActive(false);
                i =1;
                pmAmmoMain.transform.GetChild(i).gameObject.SetActive(true);
                pmAmmoMain.GetComponent<TextMeshProUGUI>().text = player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.weapon.currentAmmo.ToString() + " / " + player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.weapon.totalAmmo.ToString();

            }
            else if(player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.type == WeaponType.SNIPER)
            {
                lastIndex = i;
                pmAmmoMain.transform.GetChild(lastIndex).gameObject.SetActive(false);
                i = 3;
                pmAmmoMain.transform.GetChild(i).gameObject.SetActive(true);
                pmAmmoMain.GetComponent<TextMeshProUGUI>().text = player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.weapon.currentAmmo.ToString() + " / " + player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.weapon.totalAmmo.ToString();

            }
            else if(player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.type == WeaponType.SHOTGUN)
            {
                lastIndex = i;
                pmAmmoMain.transform.GetChild(lastIndex).gameObject.SetActive(false);
                i = 0;
                pmAmmoMain.transform.GetChild(i).gameObject.SetActive(true);
                pmAmmoMain.GetComponent<TextMeshProUGUI>().text = player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.weapon.currentAmmo.ToString() + " / " + player.transform.GetChild(1).GetComponent<ActiveWeapon>().mainWeaponObject.weapon.totalAmmo.ToString();
            }
        }
        else if (player.transform.GetChild(1).GetComponent<ActiveWeapon>().secondaryIsActive)
        {
            lastIndex = i;
            pmAmmoSecondary.transform.GetChild(lastIndex).gameObject.SetActive(false);
            i = 0;
            pmAmmoSecondary.transform.GetChild(i).gameObject.SetActive(true);
            pmAmmoSecondary.GetComponent<TextMeshProUGUI>().text = player.transform.GetChild(1).GetComponent<ActiveWeapon>().secondaryWeaponObject.weapon.currentAmmo.ToString() + " / " + player.transform.GetChild(1).GetComponent<ActiveWeapon>().secondaryWeaponObject.weapon.totalAmmo.ToString();
        }
    }
    public void GetPlayerData(GameObject player)
    {
        this.player = player;
    }
}
