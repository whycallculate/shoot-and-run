using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Unity.PlasticSCM.Editor.WebApi;
public enum ShootState
{
    IDLE,
    SHOOTING,
    RELOADING,
    
}
public class Weapons : MonoBehaviour
{
    [SerializeField] ShootState state;
    [SerializeField] ParticleSystem muzzle;
    [SerializeField] Animator anim;
    [SerializeField] WeaponType type;
    [SerializeField] string weaponName;
    [SerializeField] int ammo;
    [SerializeField] float firerate;
    [SerializeField] float recoil;
    [SerializeField] bool isAutomatic;
    [SerializeField] float reloadTime;
    [SerializeField] string bullet;
    [SerializeField] Transform firePoint;
    [SerializeField] Light weaponBarrel;
    WeaponBase weapon;
    PhotonView pw;
    bool notShooting = false;
    bool isReloading = false;

    private void Awake()
    {
        pw =GetComponent<PhotonView>();
        
        if(type == WeaponType.PISTOL)
        {
            Pistol pistol = new Pistol(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePoint);
            weapon =pistol;
        }
        else if(type == WeaponType.SMG) 
        {
            Smg smg = new Smg(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePoint);
            weapon = smg;
        }
        else if(type == WeaponType.RIFLE)
        {
            Rifle rifle = new Rifle(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePoint);
            weapon = rifle;
        }
        else if(type == WeaponType.SHOTGUN)
        {
            Shotgun shotgun = new Shotgun(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePoint);
            weapon = shotgun;
        }

    }
    private void Update()
    {
        state = ShootState.IDLE;

        if (pw.IsMine)
        {
            if(!notShooting)
            {
                StartCoroutine(ShootOnGame());

            }
            if (!isReloading)
            {
                StartCoroutine(ReloadOnGame());
            }
            StateAnimUpdate();
        }
    }

    public void StateAnimUpdate()
    {
        if(state == ShootState.IDLE)
        {
            anim.SetBool("Reloading", false);
            anim.SetBool("Shooting", false);
            weaponBarrel.enabled = false ;

            muzzle.Stop();

        }
        else if(state == ShootState.SHOOTING)
        {
            weaponBarrel.enabled = true;
            muzzle.Play();
            anim.SetBool("Reloading", false);
            anim.SetBool("Shooting", true);
            
        }
        else if(state == ShootState.RELOADING)
        {
            anim.SetBool("Reloading", true);
            anim.SetBool("Shooting", false) ;
            weaponBarrel.enabled = false;
            muzzle.Stop();

        }
    }
    IEnumerator ReloadOnGame()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(weapon.currentAmmo != weapon.maxAmmo)
            {
                isReloading = true;
                state = ShootState.RELOADING;
                yield return new WaitForSeconds(1f);
                weapon.Reload();
                isReloading = false;

            }
            
            
        }
    }
    IEnumerator ShootOnGame()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(weapon.currentAmmo == weapon.maxAmmo || weapon.currentAmmo > 0)
            {
                state = ShootState.SHOOTING;
                notShooting = true;
                weapon.Shoot();
                yield return new WaitForSeconds(weapon.fireRate);
                notShooting = false;

            }
            else if(weapon.currentAmmo <= 0)
            {
                state = ShootState.RELOADING;
                yield return new WaitForSeconds(1f);
                weapon.Reload();
            }


        }
        
    }
}

class Rifle : WeaponBase
{
    public Rifle(string weaponName,int ammo,float firerate,float recoil,bool isAutomatic,float reloadTime, string bullet,Transform firePoint)
    {
        base.weaponName = weaponName;
        base.weaponType = WeaponType.RIFLE;
        base.maxAmmo = ammo;
        base.fireRate = firerate;
        base.recoil = recoil;
        base.isAutomatic = isAutomatic;
        base.reloadTime = reloadTime;
        base.bullet = bullet;
        base.firePoint = firePoint;
        base.currentAmmo = base.maxAmmo;

    }

    public override void Reload()
    {
        
    }

    public override void Shoot()
    {


        GameObject bullet = PhotonNetwork.Instantiate(Path.Combine("bullet", base.bullet), firePoint.position, firePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * fireRate, ForceMode.Impulse);

        GameObject.Destroy(bullet, 5f);
    }
}
class Smg : WeaponBase
{
    public Smg(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePoint)
    {
        base.weaponName = weaponName;
        base.weaponType = WeaponType.SMG;
        base.maxAmmo = ammo;
        base.fireRate = firerate;
        base.recoil = recoil;
        base.isAutomatic = isAutomatic;
        base.reloadTime = reloadTime;
        base.bullet = bullet;
        base.firePoint = firePoint;
    }

    public override void Reload()
    {

    }

    public override void Shoot()
    {
        GameObject bullet = PhotonNetwork.Instantiate(Path.Combine("bullet", base.bullet), firePoint.position, firePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * fireRate, ForceMode.Impulse);

        GameObject.Destroy(bullet, 5f);
    }
}
class Pistol : WeaponBase
{
    public Pistol(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePoint)
    {
        base.weaponName = weaponName;
        base.weaponType = WeaponType.PISTOL;
        base.maxAmmo = ammo;
        base.fireRate = firerate;
        base.recoil = recoil;
        base.isAutomatic = isAutomatic;
        base.reloadTime = reloadTime;
        base.bullet = bullet;
        base.firePoint = firePoint;
    }

    public override void Reload()
    {

    }

    public override void Shoot()
    {
        Debug.Log("Shoot Shotgun");
    }
}
class Shotgun : WeaponBase
{
    public Shotgun(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePoint)
    {
        base.weaponName = weaponName;
        base.weaponType = WeaponType.SHOTGUN;
        base.maxAmmo = ammo;
        base.fireRate = firerate;
        base.recoil = recoil;
        base.isAutomatic = isAutomatic;
        base.reloadTime = reloadTime;
        base.bullet = bullet;
        base.firePoint = firePoint;
        currentAmmo = maxAmmo;
    }

    public override void Reload()
    {
        currentAmmo = maxAmmo;
    }

    public override void Shoot()
    {
        int pelletCount = 7; // Ateşlenecek mermi sayısı
        float spreadAngle = 5f; // Mermilerin yayılma açısı

        for (int i = 0; i < pelletCount; i++)
        {
            // Rastgele bir açı hesapla
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            Quaternion randomRotation = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(randomX, randomY, 0));

            // Mermiyi oluştur ve rastgele açıyla fırlat
            GameObject bullet = PhotonNetwork.Instantiate(Path.Combine("bullet", base.bullet), firePoint.position, randomRotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(randomRotation * Vector3.up * 175f, ForceMode.Impulse); // Mermiyi ileri doğru fırlat

            GameObject.Destroy(bullet, 0.2f);
        }
    }
}