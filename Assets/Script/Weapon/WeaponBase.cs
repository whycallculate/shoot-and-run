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

public abstract class WeaponBase
{
    public ParticleSystem[] particleEffect;
    public TrailRenderer trailRenderer;
    public string weaponName ;
    public WeaponType weaponType ;
    public int currentAmmo;
    public int maxAmmo ;
    public float fireRate;
    public float recoil ;
    public bool isAutomatic ;
    public float reloadTime;
    public string bullet;
    public Transform firePointnew;
    public Transform raycastDestination;
    public int weaponDamage;


    public abstract void Shoot();

    public abstract void Reload();



}

