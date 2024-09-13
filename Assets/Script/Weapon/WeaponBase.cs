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
    //Burada belirli indexlerle hata dondurecegiz.
    //Ornek olarak clipsammo 0`a ulastigi vakit ' 3 ' hatasini donderecek ve biz gerekli islemlerimizi ona gore yapacagiz.
    public int returnWarningIndex;

    //Particle effect islemleri 
    public ParticleSystem[] particleEffect;
    public TrailRenderer trailRenderer;
    public Vector3 hitpoint;
    public Vector3 hitnormal;
    public int layerAndTagIndex;

    //Silah Temel ozellikleri
    public string weaponName ;
    public WeaponType weaponType ;
    public float fireRate;
    public float recoil ;
    public bool isAutomatic ;
    public float reloadTime;
    public string bullet;
    public int weaponDamage;

    //Mermi islemleri 
    public int currentAmmo;
    public int maxAmmo;
    public int clipsAmmo;
    public int totalAmmo;

    //Raycast ve Photon islemleri.
    public Transform firePointnew;
    public Transform raycastDestination;
    public PhotonView pw;

    public abstract void GetPhotonView(PhotonView pw);
    public abstract void Shoot();

    public abstract void Reload();

    public abstract void AddBullet();

}

