using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using System.Threading;
public enum ShootState
{
    IDLE,
    SHOOTING,
    RELOADING,
    
}
//TEST ASAMASINDA BAZI MERMILER DESTROYLANMIYOR KONTROL EDILECEK..!!!
public class Weapons : MonoBehaviour, IPunObservable
{
    [Header("IK Left Hand")]
    [SerializeField] Transform leftHandTarget;
    [SerializeField] TwoBoneIKConstraint leftHandRig;

    [Header("Weapon Info")]
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
    CinemachineImpulseSource recoilShake;
    WeaponBase weapon;
    PhotonView pw;
    bool notShooting = false;

    private void Awake()
    {
        pw =GetComponent<PhotonView>();
        if (pw.IsMine)
        {
            recoilShake = GetComponent<CinemachineImpulseSource>();
        }
        
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
            Rifle rifle = new Rifle(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePoint,pw);
            weapon = rifle;
        }
        else if(type == WeaponType.SHOTGUN)
        {
            Shotgun shotgun = new Shotgun(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePoint);
            weapon = shotgun;
        }
        SetLeftHandIK();
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
            if (Input.GetKeyDown(KeyCode.R))
            {
                
                StartCoroutine(ReloadOnGame());
            }
            StateAnimUpdate(state);
        }
        
    }

    public void StateAnimUpdate(ShootState state)
    {
        if(state == ShootState.IDLE)
        {
            anim.SetBool("Reloading", false);
            anim.SetBool("Shooting", false);
            weaponBarrel.enabled = false ;
            muzzle.Stop();

        }
        if(state == ShootState.SHOOTING)
        {
            if(weapon.weaponType == WeaponType.SHOTGUN)
            {
                pw.RPC("GetSFXVolumeRPC", RpcTarget.All,3);
            }
            else if(weapon.weaponType == WeaponType.RIFLE)
            {
                pw.RPC("GetSFXVolumeRPC", RpcTarget.All, 0);

            }


            weaponBarrel.enabled = true;
            RecoilShake();
            muzzle.Play();
            anim.SetBool("Reloading", false);
            anim.SetBool("Shooting", true);
            
        }
        if(state == ShootState.RELOADING)
        {
            anim.SetBool("Reloading", true);
            weaponBarrel.enabled = false;
            muzzle.Stop();

        }
    }
    IEnumerator ReloadOnGame()
    {
        if (weapon.currentAmmo != weapon.maxAmmo)
        {
            
            state = ShootState.RELOADING;
            yield return new WaitForSeconds(1f);
            weapon.Reload();
            

        }
    }
    IEnumerator ShootOnGame()
    {
        if (!weapon.isAutomatic)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (weapon.currentAmmo == weapon.maxAmmo || weapon.currentAmmo > 0)
                {
                    state = ShootState.SHOOTING;
                    notShooting = true;
                    weapon.Shoot();
                    yield return new WaitForSeconds(weapon.fireRate);
                    notShooting = false;

                }
                else if (weapon.currentAmmo <= 0)
                {
                    state = ShootState.RELOADING;
                    yield return new WaitForSeconds(1f);
                    weapon.Reload();
                }
            }
        }
        else if (weapon.isAutomatic)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if(weapon.currentAmmo == weapon.maxAmmo || weapon.currentAmmo > 0)
                {
                    state = ShootState.SHOOTING;
                    notShooting = true;
                    weapon.Shoot();
                    yield return new WaitForSeconds(weapon.fireRate);
                    notShooting = false;
                }
                else if (weapon.currentAmmo <= 0)
                {
                    state = ShootState.RELOADING;
                    yield return new WaitForSeconds(1f);
                    weapon.Reload();
                }
            }
        }
        
    }
    public void RecoilShake()
    {
        if (pw.IsMine)
        {
            recoilShake.GenerateImpulse();

        }

    }
    public void SetLeftHandIK()
    {
        
        leftHandRig.data.target = leftHandTarget;
        AimState.Instance.rig.Build();
    }
    [PunRPC]
    public void GetSFXVolumeRPC(int sound)
    {
        SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON, sound, 0.4f);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(state);
        }
        else
        {
            if(!pw.IsMine)
            {
                StateAnimUpdate((ShootState)stream.ReceiveNext());
            }
        }

    }



}

class Rifle : WeaponBase
{
    PhotonView pw;
    public Rifle(string weaponName,int ammo,float firerate,float recoil,bool isAutomatic,float reloadTime, string bullet,Transform firePoint,PhotonView pwMine)
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
        base.currentAmmo = maxAmmo;
        pw = pwMine;

    }

    public override void Reload()
    {
        base.currentAmmo = maxAmmo;
    }

    public override void Shoot()
    {


        int pelletCount = 1; 
        float spreadAngle = 1f;
        currentAmmo--;

        for (int i = 0; i < pelletCount; i++)
        {
            // Rastgele bir açı hesapla
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            Quaternion randomRotation = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(randomX, randomY, 0));
            // Mermiyi oluştur ve rastgele açıyla fırlat
            GameObject bullet = PhotonNetwork.Instantiate(Path.Combine("bullet", base.bullet), firePoint.position, randomRotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(randomRotation * Vector3.up * 375f, ForceMode.Impulse); // Mermiyi ileri doğru fırlat
            pw.RPC("DestroyOnBullet", RpcTarget.All, bullet);
        }
    }
    [PunRPC]
    public void DestroyOnBullet(GameObject bullet)
    {
        Thread.Sleep(200);
        GameObject.Destroy(bullet);
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

    }
    public IEnumerator DestroySelfBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(0.4f);
        PhotonNetwork.Destroy(bullet);

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
    public IEnumerator DestroySelfBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(0.4f);
        PhotonNetwork.Destroy(bullet);

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
        float spreadAngle = 2f; // Mermilerin yayılma açısı
        currentAmmo--;

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
            DestroySelfBullet(bullet);
        }
    }
    public IEnumerator DestroySelfBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(0.4f);
        PhotonNetwork.Destroy(bullet);

    }
}