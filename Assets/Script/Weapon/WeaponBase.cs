using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    EMPTY,
    RIFLE,
    SMG,
    SHOTGUN,
    PISTOL,
    SNIPER

}

public abstract class WeaponBase
{
    public ParticleSystem[] particleEffect;
    public TrailRenderer trailRenderer;
    public Vector3 hitpoint;
    public Vector3 hitnormal;
    public int layerAndTagIndex;

    public string weaponName ;
    public WeaponType weaponType ;
    public int currentAmmo;
    public int maxAmmo ;
    public float fireRate;
    public float recoil ;
    public bool isAutomatic ;
    public float reloadTime;
    public string bullet;
    public int weaponDamage;

    public Transform firePointnew;
    public Transform raycastDestination;
    public PhotonView pw;


    public abstract void Shoot();

    public abstract void Reload();



}

