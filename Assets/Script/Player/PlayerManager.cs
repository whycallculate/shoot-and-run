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
    float maxHP = 100;

    int currentWeapon = 0;
    int nextWeapon;
    [SerializeField] Weapons[] weaponList;
    [SerializeField] Data playerData;
    [SerializeField] AimState playerAim;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject player;
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
            gameObject.transform.GetChild(1).GetComponent<AudioSource>().enabled = false;

        }


    }
    private void Update()
    {
        if (pw.IsMine)
        {
        }
    }


    public IEnumerator IsDeath()
    {
        if(currentHP <= 0)
        {

            playerMovement.animator.SetBool("Death", true);



            yield return new WaitForSeconds(10f);
            PhotonNetwork.Destroy(gameObject);

        }
    }


    public void TakeDamage(float damage)
    {
        pw.RPC("TakeDamageRPC", RpcTarget.All,damage);
        TakeDamageMethod(damage);
        StartCoroutine(IsDeath());

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
