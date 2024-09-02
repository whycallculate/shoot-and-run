using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Weapons weaponPrefab;

    private void OnTriggerEnter(Collider other)
    {
        ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();
        Debug.Log("OnTriggerEnter");
        Debug.Log(other.name);
        if(activeWeapon)
        {
            Debug.Log("activeWeapon");


                GameObject newWeapon = PhotonNetwork.Instantiate(Path.Combine("Weapon", weaponPrefab.name), weaponPrefab.transform.position, weaponPrefab.transform.rotation);
                newWeapon.transform.SetParent(activeWeapon.weaponParent);
                newWeapon.transform.localPosition = Vector3.zero;
                newWeapon.transform.localScale = Vector3.one;
                newWeapon.transform.localRotation = Quaternion.identity;
                activeWeapon.Equip(newWeapon);
            

        }
    }
}
