using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActiveWeapon : MonoBehaviour,IPunObservable
{
    [SerializeField] Weapons[] mainWeapon;
    public Weapons mainWeaponObject;
    int setMainIndex;
    public bool mainIsActive;

    [SerializeField] Weapons[] secondaryWeapon;
    public Weapons secondaryWeaponObject;
    int setSecondaryIndex;
    public bool secondaryIsActive;

    [SerializeField] Weapons[] meeleWeapon;

    int[] setIndex;
    int[] getIndex;



    [SerializeField] Transform crossHairTarget;
    [SerializeField] Animator playerAnim;
    [SerializeField] public Transform weaponLeftGrip;
    [SerializeField] public Transform weaponRightGrip;
    public Transform weaponParent;
    public AimState activeAimState;

    public Animator anim;
    public Animator rigController;



    public PhotonView pw;
    // Start is called before the first frame update
    void Awake()
    {
        pw = GetComponent<PhotonView>();
        setIndex = new int[2];
        getIndex = new int[2];
        if (pw.IsMine)
        {

            anim = GetComponent<Animator>();

            EquipMainWeapon(3);
            EquipSecondaryWeapon(0);
        }


    }
    private void Update()
    {
        Debug.Log(secondaryIsActive);
        Debug.Log(mainIsActive);


        if (pw.IsMine)
        {

            
            if (mainWeaponObject && secondaryWeaponObject)
            {
                if (mainIsActive && !secondaryIsActive)
                {
                    if (!mainWeaponObject.notShooting)
                    {
                        StartCoroutine(mainWeaponObject.ShootMain());

                    }
                    if (Input.GetKeyDown(KeyCode.R))
                    {

                        StartCoroutine(mainWeaponObject.ReloadOnGame());
                    }
                }
                if (secondaryWeaponObject && !mainIsActive)
                {
                    if (!secondaryWeaponObject.notShooting)
                    {
                        StartCoroutine(secondaryWeaponObject.ShootSecondary());

                    }
                    if (Input.GetKeyDown(KeyCode.R))
                    {

                        StartCoroutine(secondaryWeaponObject.ReloadOnGame());
                    }
                }
            }
            GetWeaponOnHand();


        }

    }


    public void GetWeaponOnHand()
    {
        if (pw.IsMine)
        {
            if (mainWeaponObject)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) && !mainIsActive && !secondaryIsActive)
                {

                    mainIsActive = true;
                    secondaryIsActive = false;
                    mainWeaponObject.CheckWeapon();
                    rigController.Play("equip" + mainWeaponObject.weaponName);
                    string equipName = "equip" + mainWeaponObject.weaponName;
                    pw.RPC("PlayEquipMainWeapon" ,RpcTarget.OthersBuffered, equipName);

                }
                else if (Input.GetKeyDown(KeyCode.Alpha1) && mainIsActive && !secondaryIsActive)
                {
                    mainIsActive = false;
                    secondaryIsActive = false;
                    mainWeaponObject.CheckWeapon();
                }
            }
            if (secondaryWeaponObject)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2) && !secondaryIsActive && !mainIsActive)
                {
                    mainIsActive = false;
                    secondaryIsActive = true;
                    secondaryWeaponObject.CheckWeapon();

                    rigController.Play("equip" + secondaryWeaponObject.weaponName);
                    string equipName = "equip" + secondaryWeaponObject.weaponName;
                    pw.RPC("PlayEquipSecondaryWeapon", RpcTarget.OthersBuffered, equipName);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && secondaryIsActive && !mainIsActive)
                {
                    mainIsActive = false;
                    secondaryIsActive = false;
                    secondaryWeaponObject.CheckWeapon();
                }
            }


        }
        else
        {
            return;
        }
    }
    [PunRPC]
    public void PlayEquipMainWeapon(string i)
    {
        if(!pw.IsMine)
        {
            rigController.Play(i);
        }
    }
    [PunRPC]
    public void PlayEquipSecondaryWeapon(string i)
    {
        if (!pw.IsMine)
        {
            rigController.Play(i);
        }
    }

    public void EquipMainWeapon(int mainWeaponIndex)
    {
        if (pw.IsMine)
        {
            
            if (mainWeaponIndex == 0 && mainWeapon[mainWeapon.Length - 1].gameObject.activeSelf == true)
            {
                mainWeapon[mainWeapon.Length - 1].gameObject.SetActive(false);
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);
            }
            else if(mainWeaponIndex == 0)
            {
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);
            }
            
                 
            
            if (mainWeaponIndex < mainWeapon.Length && mainWeaponIndex != 0)
            {
                mainWeapon[mainWeaponIndex - 1].gameObject.SetActive(false);
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);
            }
            if (mainWeaponIndex == mainWeapon.Length)
            {
                mainWeapon[mainWeaponIndex - 1].gameObject.SetActive(false);
                mainWeaponIndex = 0;
                setMainIndex = 0;
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);

            }
            mainWeaponObject = mainWeapon[mainWeaponIndex];
            setMainIndex++;
            rigController.Play("equip" + mainWeapon[mainWeaponIndex].weaponName);
            pw.RPC("EquipMainWeaponOtherPlayer",RpcTarget.OthersBuffered, mainWeaponIndex);
        }




    }
    [PunRPC]
    public void EquipMainWeaponOtherPlayer(int mainWeaponIndex)
    {
        if (!pw.IsMine)
        {
            if (mainWeaponIndex == 0 && mainWeapon[mainWeapon.Length - 1].gameObject.activeSelf == true)
            {
                mainWeapon[mainWeapon.Length - 1].gameObject.SetActive(false);
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);

            }
            else if (mainWeaponIndex == 0)
            {
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);
            }
            if (mainWeaponIndex < mainWeapon.Length && mainWeaponIndex != 0)
            {
                mainWeapon[mainWeaponIndex - 1].gameObject.SetActive(false);
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);

            }
            if (mainWeaponIndex == mainWeapon.Length)
            {
                mainWeapon[mainWeaponIndex - 1].gameObject.SetActive(false);
                mainWeaponIndex = 0;
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);

            }
            rigController.Play("equip" + mainWeapon[mainWeaponIndex].weaponName);
        }



    }
    public void EquipSecondaryWeapon(int secondaryWeaponIndex)
    {
        if (pw.IsMine)
        {

            if (secondaryWeaponIndex == 0 && secondaryWeapon[secondaryWeapon.Length - 1].gameObject.activeSelf == true)
            {
                secondaryWeapon[mainWeapon.Length - 1].gameObject.SetActive(false);
            }
            else if (secondaryWeaponIndex == 0)
            {
                secondaryWeapon[secondaryWeaponIndex].gameObject.SetActive(true);
            }
            if (secondaryWeaponIndex < mainWeapon.Length && secondaryWeaponIndex != 0)
            {
                secondaryWeapon[secondaryWeaponIndex - 1].gameObject.SetActive(false);
            }
            if (secondaryWeaponIndex == secondaryWeapon.Length)
            {
                secondaryWeapon[secondaryWeaponIndex - 1].gameObject.SetActive(false);
                secondaryWeaponIndex = 0;
                setSecondaryIndex = 0;

                secondaryWeapon[secondaryWeaponIndex].gameObject.SetActive(true);

            }

            secondaryWeaponObject = secondaryWeapon[secondaryWeaponIndex];
            setSecondaryIndex++;
            rigController.Play("equip" + secondaryWeapon[secondaryWeaponIndex].weaponName);
            
            pw.RPC("EquipSecondaryWeaponOtherPlayer",RpcTarget.OthersBuffered, secondaryWeaponIndex);
        }

    }
    [PunRPC]
    public void EquipSecondaryWeaponOtherPlayer(int secondaryWeaponIndex)

    {
        if (!pw.IsMine)
        {
            if (secondaryWeaponIndex == 0 && secondaryWeapon[secondaryWeapon.Length - 1].gameObject.activeSelf == true)
            {
                secondaryWeapon[secondaryWeapon.Length - 1].gameObject.SetActive(false);
            }
            else if (secondaryWeaponIndex == 0)
            {
                secondaryWeapon[secondaryWeaponIndex].gameObject.SetActive(true);
            }
            if (secondaryWeaponIndex < secondaryWeapon.Length && secondaryWeaponIndex != 0)
            {
                secondaryWeapon[secondaryWeaponIndex - 1].gameObject.SetActive(false);
            }
            if (secondaryWeaponIndex == secondaryWeapon.Length)
            {
                secondaryWeapon[secondaryWeaponIndex - 1].gameObject.SetActive(false);
                secondaryWeaponIndex = 0;
                secondaryWeapon[secondaryWeaponIndex].gameObject.SetActive(true);

            }
            rigController.Play("equip" + secondaryWeapon[secondaryWeaponIndex].weaponName);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
