using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    EMPTY,
    RIFLE,
    SMG,
    SHOTGUN,
    PISTOL

}
public enum BulletTpye
{
    EMPTY,
    RIFLE_AMMO,
    SMG_AMMO,
    SHOTGUN_AMMO,
    PISTOL_AMMO
}
public abstract class WeaponBase
{

    public string weaponName ;
    public WeaponType weaponType ;
    public int currentAmmo;
    public int maxAmmo ;
    public float fireRate;
    public float recoil ;
    public bool isAutomatic ;
    public float reloadTime;
    public string bullet;
    public Transform firePoint;


    public abstract void Shoot();

    public abstract void Reload();



}

