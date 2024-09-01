using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using System.Threading;
using UnityEngineInternal;
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

    [SerializeField] GameObject[] particleEffect;
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
    [SerializeField] Transform firePointnew;
    [SerializeField] Transform raycastDestination;
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
            Pistol pistol = new Pistol(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePoint, firePointnew);
            weapon =pistol;
        }
        else if(type == WeaponType.SMG) 
        {
            Smg smg = new Smg(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePoint, firePointnew);
            weapon = smg;
        }
        else if(type == WeaponType.RIFLE)
        {
            Rifle rifle = new Rifle(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePoint,pw, firePointnew,particleEffect, raycastDestination);
            weapon = rifle;
        }
        else if(type == WeaponType.SHOTGUN)
        {
            Shotgun shotgun = new Shotgun(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePoint, firePointnew);
            weapon = shotgun;
        }
        //SetLeftHandIK();
    }
    private void Update()
    {
        state = ShootState.IDLE;
        if (pw.IsMine)
        {
            firePointnew.LookAt(AimState.Instance.aimPos.transform);

        }
        if (pw.IsMine)
        {
            if(!notShooting)
            {
                if (AimState.Instance.isAiming)
                {
                    StartCoroutine(ShootOnGame());
                }
                else
                    return;
                
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
                    AimState.Instance.yAxis.Value -= weapon.recoil;
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
        //AimState.Instance.rig.Build();
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
    public Rifle(string weaponName,int ammo,float firerate,float recoil,bool isAutomatic,float reloadTime, string bullet,Transform firePoint,PhotonView pwMine,Transform firePointnew, GameObject[] particleEffect,Transform raycastDestination)
    {
        base.particleEffect = particleEffect;
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
        base.weaponDamage = 20;
        base.firePointnew = firePointnew;
        base.raycastDestination = raycastDestination;
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
            Quaternion randomRotation = Quaternion.Euler(firePointnew.rotation.eulerAngles + new Vector3(randomX, randomY, 0));

            // Ateş noktası ile aynı rotasyonda bir ray oluştur
            Ray ray = new Ray();
            RaycastHit hit;

            ray.origin = firePointnew.position;
            ray.direction = raycastDestination.position - firePointnew.position;
            // Raycast ile çarpışmayı kontrol et
            if (Physics.Raycast(ray, out hit)) // 100f menzilini istediğin gibi ayarlayabilirsin
            {
                Debug.DrawLine(ray.origin,hit.point,Color.red,4f);

                //Debug.Log(Physics.Raycast(ray, out hit, 100f));
                //Debug.Log("Çarpma Noktası: " + hit.point); // Çarpma noktası
                Debug.Log("Çarpılan Nesne: " + hit.collider.name); // Çarpılan nesnenin ismi

                // Eğer çarpılan nesne bir oyuncuysa, hasar uygula
                if (hit.collider.CompareTag("Playerr"))
                {
                    // Burada hasar hesaplamasını ve uygulamasını yap
                    // Hedef oyuncunun health scriptine hasar gönderebilirsin
                    hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage);
                    GameObject particle = GameObject.Instantiate(particleEffect[0], hit.point, Quaternion.LookRotation(hit.normal));

                    GameObject.Destroy(particle, 0.1f);

                }

                // Mermi çarptıktan sonra görsel bir efekt oynatmak isteyebilirsin
                
                GameObject tracers =PhotonNetwork.Instantiate(Path.Combine("bullet",bullet), firePoint.position, Quaternion.LookRotation(hit.normal));
                Rigidbody rb = tracers.GetComponent<Rigidbody>();
                rb.AddForce(randomRotation * Vector3.forward * 175f, ForceMode.Impulse); // Mermiyi ileri doğru fırlat

            }
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
    public Smg(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePoint, Transform firePointnew)
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
        base.firePointnew = firePointnew;
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
    public Pistol(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePoint, Transform firePointnew)
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
        base.firePointnew = firePointnew;
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
    public Shotgun(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePoint, Transform firePointnew)
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
        base.firePointnew = firePointnew;
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