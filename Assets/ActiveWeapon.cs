using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActiveWeapon : MonoBehaviour, IPunObservable
{
    [SerializeField] Weapons[] mainWeapon;
    public Weapons mainWeaponObject;

    int setMainIndexRPC;
    int setMainIndex;
    int getIndexRPC;
    public bool mainIsActive;
    [SerializeField] Weapons[] secondaryWeapon;
    public Weapons secondaryWeaponObject;
    int setSecondaryIndexRPC;
    int setSecondaryIndex;
    int getSecondaryIndexRPC;
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
    void Start()
    {
        pw = GetComponent<PhotonView>();
        if (pw.IsMine)
        {
            anim = GetComponent<Animator>();
            Weapons newWeapons = GetComponentInChildren<Weapons>();
            if (newWeapons)
            {
                EquipMainWeapon(0);
                EquipSecondaryWeapon(0);
            }
        }


    }
    private void Update()
    {
        Debug.Log(secondaryIsActive);
        Debug.Log(mainIsActive);


        if(pw.IsMine)
        {
            if (mainWeaponObject && secondaryWeaponObject)
            {
                if (mainIsActive && !secondaryIsActive)
                {
                    if(!mainWeaponObject.notShooting)
                    {
                        StartCoroutine(mainWeaponObject.ShootMain());

                    }
                }
                if (secondaryWeaponObject && !mainIsActive)
                {
                    if (!mainWeaponObject.notShooting)
                    {
                        StartCoroutine(secondaryWeaponObject.ShootSecondary());

                    }

                }
            }
            GetWeaponOnHand();
            if (Input.GetKeyDown(KeyCode.B))
            {

                EquipMainWeapon(setMainIndex);

            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                EquipSecondaryWeapon(setSecondaryIndex);
            }

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


    public void EquipMainWeapon(int mainWeaponIndex)
    {
        if (pw.IsMine)
        {
            
            if (mainWeaponIndex == 0 && mainWeapon[mainWeapon.Length -1].gameObject.activeSelf == true)
            {
                mainWeapon[mainWeapon.Length -1].gameObject.SetActive(false);
                setMainIndexRPC = mainWeaponIndex;
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);
            }
            if (mainWeaponIndex < mainWeapon.Length && mainWeaponIndex != 0)
            {
                mainWeapon[mainWeaponIndex - 1].gameObject.SetActive(false);
                setMainIndexRPC = mainWeaponIndex;
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);
            }
            if (mainWeaponIndex == mainWeapon.Length)
            {
                mainWeapon[mainWeaponIndex -1].gameObject.SetActive(false);
                mainWeaponIndex = 0;
                setMainIndex = 0;
                setMainIndexRPC = mainWeaponIndex;
                mainWeapon[mainWeaponIndex].gameObject.SetActive(true);

            }
            Debug.Log(mainWeaponIndex);
            mainWeaponObject = mainWeapon[mainWeaponIndex];
            setMainIndex++;
            rigController.Play("equip" + mainWeapon[mainWeaponIndex].weaponName);
        }





    }
   
    public void EquipMainWeaponOtherPlayer(int mainWeaponIndex)
    {
        if(!pw.IsMine)
        {
            if (mainWeaponIndex == 0 && mainWeapon[mainWeapon.Length - 1].gameObject.activeSelf == true)
            {
                mainWeapon[mainWeapon.Length - 1].gameObject.SetActive(false);
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

                setMainIndexRPC = 0;
            }
            rigController.Play("equip" + mainWeapon[mainWeaponIndex].weaponName);
        }



    }
    public void EquipSecondaryWeapon(int secondaryWeaponIndex)
    {
        if(pw.IsMine)
        {
            Debug.Log(secondaryWeaponIndex);

            if (secondaryWeaponIndex == 0 && secondaryWeapon[secondaryWeapon.Length - 1].gameObject.activeSelf == true)
            {
                secondaryWeapon[mainWeapon.Length - 1].gameObject.SetActive(false);
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
                setSecondaryIndexRPC = secondaryWeaponIndex;

                secondaryWeapon[secondaryWeaponIndex].gameObject.SetActive(true);

            }
            secondaryWeaponObject = secondaryWeapon[secondaryWeaponIndex];
            setSecondaryIndex++;
            rigController.Play("equip" + secondaryWeapon[secondaryWeaponIndex].weaponName);
        }

    }
    public void EquipSecondaryWeaponOtherPlayer(int secondaryWeaponIndex)
    {
        if (!pw.IsMine)
        {
            if (secondaryWeaponIndex == 0 && secondaryWeapon[secondaryWeapon.Length - 1].gameObject.activeSelf == true)
            {
                secondaryWeapon[secondaryWeapon.Length - 1].gameObject.SetActive(false);
            }
            if (secondaryWeaponIndex < secondaryWeapon.Length && secondaryWeaponIndex != 0)
            {
                secondaryWeapon[secondaryWeaponIndex - 1].gameObject.SetActive(false);
            }
            if (secondaryWeaponIndex == secondaryWeapon.Length)
            {
                secondaryWeapon[secondaryWeaponIndex - 1].gameObject.SetActive(false);
                secondaryWeaponIndex = 0;
                setSecondaryIndexRPC = 0;
                secondaryWeapon[secondaryWeaponIndex].gameObject.SetActive(true);

            }
            rigController.Play("equip" + secondaryWeapon[secondaryWeaponIndex].weaponName);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

            setIndex[0] = setMainIndexRPC;
            setIndex[1] = setSecondaryIndexRPC;
            stream.SendNext(setIndex);
        }
        else
        {
            if (!pw.IsMine)
            {
                getIndex = (int[])stream.ReceiveNext();
                EquipMainWeaponOtherPlayer(getIndex[0]);

                EquipSecondaryWeaponOtherPlayer(getIndex[1]);
            }

        }

    }
}
