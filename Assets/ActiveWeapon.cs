using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] Transform crossHairTarget;
    [SerializeField] Animator playerAnim;
    Weapons weapon;
    PhotonView pw;
    // Start is called before the first frame update
    void Start()
    {
        pw = GetComponent<PhotonView>();
        if (pw.IsMine)
        {
            Weapons newWeapons = GetComponentInChildren<Weapons>();
            if (newWeapons)
            {
                Equip(newWeapons);
            }
        }


    }

    public void Equip(Weapons weapon)
    {
        this.weapon = weapon;
        this.weapon.raycastDestination = crossHairTarget;
        this.weapon.anim = playerAnim;
    }
}
