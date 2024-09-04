using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WeaponPickup : MonoBehaviourPun
{
    public Weapons weaponPrefab;
    public PhotonView pw;

    private void Awake()
    {
        pw = GetComponent<PhotonView>();


    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (pw.IsMine)
        {
            Debug.Log("CALISTI MI");
            GameObject newWeapon = PhotonNetwork.Instantiate(Path.Combine("Weapon", weaponPrefab.name), weaponPrefab.transform.position, weaponPrefab.transform.rotation);
            PhotonView newPw = newWeapon.GetComponent<PhotonView>();
            ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();

            Debug.Log(newPw.ViewID + " OnTriggerEnter");
            PhotonView activeWeaponPw = activeWeapon.GetComponent<PhotonView>();
            SetWeaponForPlayer(newWeapon, activeWeapon);
            pw.RPC("SetWeaponForPlayer_RPC", RpcTarget.Others, newPw.ViewID,activeWeaponPw.ViewID);
            activeWeapon.Equip(newWeapon); // Silahı donat

        }

        

    }

    private void SetWeaponForPlayer(GameObject weapon, ActiveWeapon activeWeapon)
    {
        weapon.transform.SetParent(activeWeapon.weaponParent);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localScale = Vector3.one;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.GetComponent<Weapons>().anim = activeWeapon.anim;
        weapon.GetComponent<Weapons>().rigController = activeWeapon.rigController;
    }

    [PunRPC]
    public void SetWeaponForPlayer_RPC(int weaponViewID,int activeWeaponPw)
    {
        Debug.Log(weaponViewID);
        // PhotonView ID'lerine göre nesneleri bul
        GameObject weapon = PhotonView.Find(weaponViewID).gameObject;
        ActiveWeapon activeWeapon = PhotonView.Find(activeWeaponPw).GetComponent<ActiveWeapon>();
        if (!pw.IsMine)
        {
            // Diğer oyuncular için silahı ayarla
            weapon.transform.SetParent(activeWeapon.weaponParent);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localScale = Vector3.one;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.GetComponent<Weapons>().anim = activeWeapon.anim;
            weapon.GetComponent<Weapons>().rigController = activeWeapon.rigController;
            activeWeapon.Equip(weapon);
        }

    }
}