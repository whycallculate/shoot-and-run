using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEditor.Rendering;
using UnityEditor;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    [Header("Player Info")]
    public float currentHP;
    float maxHP = 200;
    public float currentArmor;
    float armor;

    private const string KILLS = "Kills";
    private const string DEATHS = "Deaths";

    Player lastAtacker;

    [SerializeField] Data playerData;
    [SerializeField] AimState playerAim;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject player;
    [SerializeField] ActiveWeapon activeWeapon;
    [SerializeField] Camera mainCamera;
    public PhotonView pw;
    [SerializeField] ParticleSystem[] playerParticle;
    private void OnEnable()
    {
        currentHP = maxHP;
    }

    private void Awake()
    {
         pw = GetComponent<PhotonView>();
        if (pw.IsMine)
        {

            SoundManager.playerSfx = transform.GetComponent<AudioSource>();
            SetStatsHash();

        }
        else if (!pw.IsMine)
        {
            mainCamera.enabled = false;
            mainCamera.GetComponent<AudioListener>().enabled = false;
            mainCamera.GetComponent<CinemachineBrain>().enabled = false;
            gameObject.GetComponent<AudioSource>().enabled = false;   

        }


    }
    private void Update()
    {
        if (!pw.IsMine)
        {
            Debug.Log(currentHP);
        }
        if (pw.IsMine)
        {
            Debug.Log(currentHP);
        }
    }

    public void SetStatsHash()
    {
        if (pw.IsMine)
        {
            Hashtable stats = new Hashtable
            {
                { KILLS, 0 },
                { DEATHS, 0 },
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(stats);
        }
    }

    void OnPlayerKilled(Player killer, Player victim)
    {
        
        int currentKills = (int)PhotonNetwork.LocalPlayer.CustomProperties[KILLS];
        Hashtable killerStats = new Hashtable
        {
            { KILLS, currentKills + 1 }
        };
        killer.SetCustomProperties(killerStats);

        int currentDeath = (int)PhotonNetwork.LocalPlayer.CustomProperties[DEATHS];
        Hashtable victimStats = new Hashtable
        {
            { DEATHS, currentDeath + 1 }
        };
        victim.SetCustomProperties(victimStats);


    }



    public IEnumerator IsDeath()
    {
        if (pw.IsMine)
        {
            
            if (currentHP <= 0)
            {
                activeWeapon.mainIsActive = false;
                activeWeapon.secondaryIsActive = false;
                activeWeapon.mainWeaponObject.CheckWeapon();
                playerMovement.animator.SetBool("Death", true);
                playerMovement.rigAnimator.SetBool("SetWeight", true);
                yield return new WaitForSeconds(3f);
                playerMovement.rigAnimator.SetBool("SetWeight", false);
                playerMovement.animator.SetBool("Death", false);
                GameManager.Instance.PlayerDeath(transform.GetParentComponent<Transform>().gameObject);
                pw.RPC("isDeathRPC", RpcTarget.Others);
                currentHP = maxHP;

            }

        }

    }

    public IEnumerator IsDeath(int actorNumber)
    {
        if(pw.IsMine)
        {

            if (currentHP <= 0)
            {
                activeWeapon.mainIsActive = false;
                activeWeapon.secondaryIsActive = false;
                activeWeapon.mainWeaponObject.CheckWeapon();

                playerMovement.animator.SetBool("Death", true);
                playerMovement.rigAnimator.SetBool("SetWeight", true);

                lastAtacker = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
                OnPlayerKilled(lastAtacker, PhotonNetwork.LocalPlayer);
                Debug.Log(lastAtacker.NickName +" "+ lastAtacker.ActorNumber);

                yield return new WaitForSeconds(3f);

                playerMovement.rigAnimator.SetBool("SetWeight", false);
                playerMovement.animator.SetBool("Death", false);

                GameManager.Instance.PlayerDeath(transform.GetParentComponent<Transform>().gameObject);
                pw.RPC("isDeathRPC", RpcTarget.Others);
                currentHP = maxHP;
                
            }

        }

    }
    [PunRPC]
    public void isDeathRPC()
    {
        if (!pw.IsMine)
        {
            GameManager.Instance.PlayerDeath(transform.GetParentComponent<Transform>().gameObject);

        }
    }
    public void GiveDamage(float damage)
    {
        if (pw.IsMine)
        {
            pw.RPC("GiveDamageRPC", RpcTarget.Others, damage);
            GiveDamageMethod(damage);
        }

    }

    public void TakeDamage(float damage, int actorNumber)
    {
        if(!pw.IsMine)
        {
            Debug.Log("Current Hp TakeDamage" + currentHP);
            pw.RPC("TakeDamageRPC", RpcTarget.Others, damage, actorNumber);
            TakeDamageMethod(damage);
            StartCoroutine(IsDeath(actorNumber));
        }


    }


    [PunRPC]
    public void TakeDamageRPC(float damage, int actorNumber)
    {
        if (!this.pw.IsMine)
        {
            return;
        }
        StartCoroutine(HittingCoroutine());
        if (armor > 0)
        {
            currentArmor -= damage;
        }
        else if (armor <= 0)
        {
            currentHP -= damage;
        }
        StartCoroutine(IsDeath(actorNumber));
        
        Debug.Log(currentHP +PhotonNetwork.NickName);
    }
    [PunRPC]
    public void GiveDamageRPC(float damage)
    {
        if (!pw.IsMine)
        {
            if (armor > 0)
            {
                currentArmor -= damage;
            }
            else if (armor <= 0)
            {
                currentHP -= damage;
            }
            StartCoroutine(IsDeath());
        }
    }
    public void TakeDamageMethod(float damage)
    {
        if (!pw.IsMine)
        {
            StartCoroutine(HittingCoroutine());
            if (armor > 0)
            {
                currentArmor -= damage;
            }
            else if (armor <= 0)
            {
                currentHP -= damage;
            }
            
            Debug.Log(currentHP + PhotonNetwork.NickName);

        }

    }
    public void GiveDamageMethod(float damage)
    {
        if (pw.IsMine)
        {
            if (armor > 0)
            {
                currentArmor -= damage;
            }
            else if (armor <= 0)
            {
                currentHP -= damage;
            }
        }
    }

    public IEnumerator HittingCoroutine()
    {
        playerAim.anim.SetBool("Hitting", true);
        yield return new WaitForSeconds(0.2f);
        playerAim.anim.SetBool("Hitting", false);

    }
}

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
        //if(stream.IsWriting)
        //{
            //stream.SendNext(currentHP);
        //}
        //else
        //{
            //if (!pw.IsMine)
            //{
                //currentHP = (float)stream.ReceiveNext();

            //}
        //}
    //}
//}
