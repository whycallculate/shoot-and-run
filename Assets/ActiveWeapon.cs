using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] Transform crossHairTarget;
    [SerializeField] Animator playerAnim;
    [SerializeField] public Transform weaponLeftGrip;
    [SerializeField] public Transform weaponRightGrip;
    public Transform weaponParent;

    public Animator anim;
    public Animator rigController;
    Weapons weapon;



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
                Equip(newWeapons);
            }
        }


    }


    public void Equip(GameObject gameObjectWeapon)
    {
        if (weapon)
        {
            PhotonNetwork.Destroy(weapon.gameObject);
        }
        if (pw.IsMine)
        {
            weapon = gameObjectWeapon.GetComponent<Weapons>();
            weapon.pw = pw;
            weapon.raycastDestination = crossHairTarget;
            rigController.Play("equip" + weapon.weaponName);
        }



    }
    public void Equip(Weapons weapon)
    {
        if (weapon)
        {
            PhotonNetwork.Destroy(weapon.gameObject);
        }
        if (pw.IsMine)
        {
            
            weapon.pw = pw;
            weapon.raycastDestination = crossHairTarget;

            rigController.Play("equip" + weapon.weaponName);
        }


    }


}
