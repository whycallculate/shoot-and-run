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

    private void OnEnable()
    {
        currentHP = maxHP;
    }

    [Header("Player Info")]
    public float currentHP;
    float maxHP = 200;
    [SerializeField] Data playerData;
    [SerializeField] AimState playerAim;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject player;
    [SerializeField] ActiveWeapon activeWeapon;
    [SerializeField] Camera mainCamera;
    PhotonView pw;
    private void Awake()
    {
        pw = GetComponent<PhotonView>();
        if (pw.IsMine)
        {

            SoundManager.playerSfx = transform.GetChild(1).GetComponent<AudioSource>();


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


    public IEnumerator IsDeath()
    {
        if(pw.IsMine)
        {
            if (currentHP <= 0)
            {
                activeWeapon.mainIsActive = false;
                activeWeapon.secondaryIsActive = false;
                activeWeapon.mainWeaponObject.CheckWeapon();
                playerMovement.animator.SetBool("Death", true);
                yield return new WaitForSeconds(5f);
                PhotonNetwork.Destroy(player.gameObject);
            }

        }

    }


    public void TakeDamage(float damage)
    {
        if(!pw.IsMine)
        {
            Debug.Log("Current Hp TakeDamage" + currentHP);
            pw.RPC("TakeDamageRPC", RpcTarget.Others, damage);
            TakeDamageMethod(damage);
            StartCoroutine(IsDeath());
        }


    }
    [PunRPC]
    public void TakeDamageRPC(float damage )
    {
        if (!pw.IsMine)
        {
            return;
        }

        StartCoroutine(HittingCoroutine());
        currentHP -= damage;
        StartCoroutine(IsDeath());

        Debug.Log(currentHP +PhotonNetwork.NickName);
    }
    public void TakeDamageMethod(float damage)
    {
        if (!pw.IsMine)
        {
            StartCoroutine(HittingCoroutine());
            currentHP -= damage;
            Debug.Log(currentHP + PhotonNetwork.NickName);

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
