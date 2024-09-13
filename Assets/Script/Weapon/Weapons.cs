using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using System.Threading;
using UnityEngineInternal;
using Unity.VisualScripting;
using Unity.Burst.CompilerServices;
public enum ShootState
{
    IDLE,
    SHOOTING,
    RELOADING,
    
}
//TEST ASAMASINDA BAZI MERMILER DESTROYLANMIYOR KONTROL EDILECEK..!!!
public class Weapons : MonoBehaviour, IPunObservable
{

    [Header("Camera")]
    CinemachineVirtualCamera virtualCamera;
    Cinemachine.CinemachineImpulseSource impulseSource;
    float time;
    public float duration;


    [Header("Weapon Info")]
    [SerializeField] ParticleSystem[] particleEffect;
    [SerializeField] ShootState state;
    [SerializeField] public Animator anim;
    [SerializeField] public WeaponType type;
    [SerializeField] public string weaponName;
    [SerializeField] int ammo;
    [SerializeField] float firerate;
    [SerializeField] float recoil;
    [SerializeField] bool isAutomatic;
    [SerializeField] float reloadTime;
    [SerializeField] string bullet;
    [SerializeField] Transform firePointnew;
    [SerializeField] public Transform raycastDestination;
    [SerializeField] Light weaponBarrel;
    [SerializeField] public AnimationClip weaponAnimation;
    CinemachineImpulseSource recoilShake;
    public Animator rigController;
    public AimState newAimState;
    public ActiveWeapon activeWeapon;
    public WeaponBase weapon;
    public PhotonView pw;
    public bool notShooting = false;

    private void Awake()
    {
        pw = GetComponent<PhotonView>();
        if (pw.IsMine)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
            recoilShake = GetComponent<CinemachineImpulseSource>();
            virtualCamera = newAimState.virtualCamera;
        }

        if (type == WeaponType.PISTOL)
        {
            Pistol pistol = new Pistol(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePointnew, particleEffect, raycastDestination);
            weapon = pistol;
        }
        else if (type == WeaponType.SMG)
        {
            Smg smg = new Smg(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePointnew, particleEffect, raycastDestination);
            weapon = smg;
        }
        else if (type == WeaponType.RIFLE)
        {
            Rifle rifle = new Rifle(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePointnew, particleEffect, raycastDestination);
            weapon = rifle;
        }
        else if (type == WeaponType.SHOTGUN)
        {
            Shotgun shotgun = new Shotgun(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePointnew, particleEffect, raycastDestination);
            weapon = shotgun;
        }
        else if (type == WeaponType.SNIPER)
        {
            Sniper sniper = new Sniper(weaponName, ammo, firerate, recoil, isAutomatic, reloadTime, bullet, firePointnew, particleEffect, raycastDestination);
            weapon = sniper;
        }

    }
    private void Start()
    {
        if (pw.IsMine)
        {
            weapon.GetPhotonView(pw);
            Debug.Log("Pw isMine True Weapon");
        }
    }
    private void Update()
    {
        state = ShootState.IDLE;
        if (pw.IsMine)
        {
            Debug.Log(weapon.currentAmmo);
            firePointnew.LookAt(newAimState.aimPos.transform);
            if (time > 0)
            {
                newAimState.yAxis.Value -= (weapon.recoil * Time.deltaTime) / duration;
                time -= Time.deltaTime;
            }
        }
        if (pw.IsMine)
        {
            if (!notShooting)
            {
                StartCoroutine(CheckWeapon());

            }

            StateAnimUpdate(state);
        }

    }


    public void HoldingWeapon()
    {
        if (weapon.weaponType == WeaponType.PISTOL)
        {
            anim.SetBool("PistolHand", true);
            anim.SetBool("RifleHand", false);
        }
        else
        {
            anim.SetBool("RifleHand", true);
            anim.SetBool("PistolHand", false);
        }
    }
    public IEnumerator CheckWeapon()
    {
        if (activeWeapon.mainIsActive && !activeWeapon.secondaryIsActive)
        {
            rigController.SetBool("holsterSecondary", false);
            rigController.SetBool("holsterWeapon", true);
            yield return new WaitForSeconds(0.1f);

            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            HoldingWeapon();
        }
        else if (!activeWeapon.mainIsActive && !activeWeapon.secondaryIsActive)
        {
            rigController.SetBool("holsterSecondary", false);
            
            rigController.SetBool("holsterWeapon", false);
            yield return new WaitForSeconds(0.1f);

            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        }
        else if (activeWeapon.secondaryIsActive && !activeWeapon.mainIsActive)
        {
            rigController.SetBool("holsterSecondary", true);
            
            rigController.SetBool("holsterWeapon", false);
            yield return new WaitForSeconds(0.1f);

            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            HoldingWeapon();
        }
        else if (!activeWeapon.secondaryIsActive && !activeWeapon.mainIsActive)
        {
            rigController.SetBool("holsterSecondary", false);
            rigController.SetBool("holsterWeapon", false);
            yield return new WaitForSeconds(0.1f);

            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        }
    }
    public void StateAnimUpdate(ShootState state)
    {
        if(state == ShootState.IDLE)
        {

            weaponBarrel.enabled = false ;

        }
        if(state == ShootState.SHOOTING)
        {
            
            weaponBarrel.enabled = true;
            RecoilShake();

            
        }
        if(state == ShootState.RELOADING)
        {
            weaponBarrel.enabled = false;
        }
    }
    public void GenerateRecoil()
    {
        time = duration;
        impulseSource.GenerateImpulse(Camera.main.transform.forward);
        rigController.Play("weaponRecoil" + weapon.weaponType.ToString(), 1, 0.0f);

    }
    public void MakeVFXShoot()
    {

        if (weapon.layerAndTagIndex == 0)
        {

            particleEffect[0].Emit(1);
            particleEffect[1].Emit(1);

            particleEffect[2].transform.position = weapon.hitpoint;
            particleEffect[2].transform.forward = weapon.hitnormal;

            particleEffect[2].Emit(1);

            particleEffect[3].transform.position = weapon.hitpoint;
            particleEffect[3].transform.forward = weapon.hitnormal;
            particleEffect[3].Emit(1);

            particleEffect[4].transform.position = weapon.hitpoint;
            particleEffect[4].transform.forward = weapon.hitnormal;
            particleEffect[4].Emit(1);

            particleEffect[5].transform.position = weapon.hitpoint;
            particleEffect[5].transform.forward = weapon.hitnormal;
            particleEffect[5].Emit(1);
            pw.RPC("ShootOtherClient", RpcTarget.Others, weapon.hitpoint, weapon.hitnormal);
        }
        else if(weapon.layerAndTagIndex == 1)
        {
            particleEffect[6].Emit(1);
            particleEffect[6].transform.position = weapon.hitpoint;
            particleEffect[6].transform.forward = weapon.hitnormal;

            particleEffect[7].Emit(1);
            particleEffect[7].transform.position = weapon.hitpoint;
            particleEffect[7].transform.forward = weapon.hitnormal;

            particleEffect[8].Emit(1);
            particleEffect[8].transform.position = weapon.hitpoint;
            particleEffect[8].transform.forward = weapon.hitnormal;

            particleEffect[9].Emit(200);
            particleEffect[9].transform.position = weapon.hitpoint;
            particleEffect[9].transform.forward = weapon.hitnormal;
            pw.RPC("ShootOtherClientBlood", RpcTarget.Others, weapon.hitpoint, weapon.hitnormal);

        }
        else if (weapon.layerAndTagIndex == 2)
        {
            particleEffect[6].Emit(5);
            particleEffect[6].transform.position = weapon.hitpoint;
            particleEffect[6].transform.forward = weapon.hitnormal;

            particleEffect[7].Emit(15);
            particleEffect[7].transform.position = weapon.hitpoint;
            particleEffect[7].transform.forward = weapon.hitnormal;

            particleEffect[8].Emit(2);
            particleEffect[8].transform.position = weapon.hitpoint;
            particleEffect[8].transform.forward = weapon.hitnormal;

            particleEffect[9].Emit(200);
            particleEffect[9].transform.position = weapon.hitpoint;
            particleEffect[9].transform.forward = weapon.hitnormal;
            pw.RPC("ShootOtherClientBlood", RpcTarget.Others, weapon.hitpoint, weapon.hitnormal);

        }
    }
    [PunRPC]
    public void ShootOtherClient(Vector3 hitpoint, Vector3 hitnormal)
    {
        if (!pw.IsMine)
        {
            particleEffect[0].Emit(1);
            particleEffect[1].Emit(1);

            particleEffect[2].transform.position = hitpoint;
            particleEffect[2].transform.forward = hitnormal;
            particleEffect[2].Emit(1);

            particleEffect[3].transform.position = hitpoint;
            particleEffect[3].transform.forward = hitnormal;
            particleEffect[3].Emit(1);

            particleEffect[4].transform.position = hitpoint;
            particleEffect[4].transform.forward = hitnormal;
            particleEffect[4].Emit(1);

            particleEffect[5].transform.position = hitpoint;
            particleEffect[5].transform.forward = hitnormal;
            particleEffect[5].Emit(1);

        }

    }

    [PunRPC]
    public void ShootOtherClientBlood(Vector3 hitpoint, Vector3 hitnormal)
    {
        if (!pw.IsMine)
        {
            particleEffect[6].Emit(1);
            particleEffect[6].transform.position = hitpoint;
            particleEffect[6].transform.forward = hitnormal;

            particleEffect[7].Emit(1);
            particleEffect[7].transform.position = hitpoint;
            particleEffect[7].transform.forward = hitnormal;

            particleEffect[8].Emit(1);
            particleEffect[8].transform.position = hitpoint;
            particleEffect[8].transform.forward = hitnormal;

            particleEffect[9].Emit(200);
            particleEffect[9].transform.position = hitnormal;
            particleEffect[9].transform.forward = hitnormal;
        }

    }
    public IEnumerator ReloadOnGame()
    {
        if (weapon.currentAmmo != weapon.maxAmmo)
        {
            if(weapon.returnWarningIndex == 0)
            {
                if (weapon.weaponType == WeaponType.SHOTGUN)
                {
                    SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON_RELOAD, 3, 0.4f);
                }
                else if (weapon.weaponType == WeaponType.RIFLE)
                {
                    SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON_RELOAD, 0, 0.4f);

                }
                else if (weapon.weaponType == WeaponType.SMG)
                {
                    SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON_RELOAD, 4, 0.4f);

                }
                else if (weapon.weaponType == WeaponType.PISTOL)
                {
                    SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON_RELOAD, 1, 0.4f);

                }
                else if (weapon.weaponType == WeaponType.SNIPER)
                {
                    SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON_RELOAD, 2, 0.4f);

                }
                state = ShootState.RELOADING;
                rigController.Play("weaponReload" + weapon.weaponType.ToString(), 2, 0.0f);
                pw.RPC("ReloadOnOtherPlayer", RpcTarget.Others);
                yield return new WaitForSeconds(1f);
                weapon.Reload();
            }
            else if(weapon.returnWarningIndex == 1)
            {
                SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON_RELOAD, 5, 0.4f);
            }
        }
    }

    [PunRPC]
    public void ReloadOnOtherPlayer()
    {
        if (!pw.IsMine)
        {
            rigController.Play("weaponReload" + weapon.weaponType.ToString(), 2, 0.0f);
        }
    }
    public IEnumerator ShootMain()
    {
        if (pw.IsMine)
        {
            if(!notShooting)
            {
                if (!weapon.isAutomatic)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (weapon.currentAmmo == weapon.maxAmmo || weapon.currentAmmo > 0)
                        {
                            GenerateRecoil();
                            ShootSoundEffect();
                            state = ShootState.SHOOTING;
                            notShooting = true;
                            weapon.Shoot();
                            MakeVFXShoot();
                            yield return new WaitForSeconds(weapon.fireRate);
                            notShooting = false;

                        }
                    }
                }
                else if (weapon.isAutomatic)
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        if (weapon.currentAmmo == weapon.maxAmmo || weapon.currentAmmo > 0)
                        {
                            GenerateRecoil();
                            ShootSoundEffect();
                            state = ShootState.SHOOTING;
                            notShooting = true;
                            weapon.Shoot();
                            MakeVFXShoot();
                            yield return new WaitForSeconds(weapon.fireRate);
                            notShooting = false;
                        }

                    }
                }
            }
            
        }
    }

    public IEnumerator ShootSecondary()
    {
        if (pw.IsMine)
        {
            if (!notShooting)
            {
                if (!weapon.isAutomatic)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (weapon.currentAmmo == weapon.maxAmmo || weapon.currentAmmo > 0)
                        {
                            GenerateRecoil();
                            ShootSoundEffect();
                            state = ShootState.SHOOTING;
                            notShooting = true;
                            weapon.Shoot();
                            MakeVFXShoot();
                            yield return new WaitForSeconds(weapon.fireRate);
                            notShooting = false;

                        }
                    }
                }
                else if (weapon.isAutomatic)
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        if (weapon.currentAmmo == weapon.maxAmmo || weapon.currentAmmo > 0)
                        {
                            GenerateRecoil();
                            ShootSoundEffect();
                            state = ShootState.SHOOTING;
                            notShooting = true;
                            weapon.Shoot();
                            MakeVFXShoot();
                            yield return new WaitForSeconds(weapon.fireRate);
                            notShooting = false;
                        }
                    }
                }
            }
        }
    }

    public void ShootSoundEffect()
    {
        if (weapon.weaponType == WeaponType.SHOTGUN)
        {
            SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON, 3, 0.4f);
            pw.RPC("GetSFXVolumeRPC", RpcTarget.Others, 3);
        }
        else if (weapon.weaponType == WeaponType.RIFLE)
        {
            SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON, 0, 0.4f);
            pw.RPC("GetSFXVolumeRPC", RpcTarget.Others, 0);

        }
        else if (weapon.weaponType == WeaponType.SMG)
        {
            SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON, 4, 0.4f);
            pw.RPC("GetSFXVolumeRPC", RpcTarget.Others, 4);

        }
        else if (weapon.weaponType == WeaponType.PISTOL)
        {
            SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON, 1, 0.4f);
            pw.RPC("GetSFXVolumeRPC", RpcTarget.Others, 1);

        }
        else if (weapon.weaponType == WeaponType.SNIPER)
        {
            SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON, 2, 0.4f);
            pw.RPC("GetSFXVolumeRPC", RpcTarget.Others, 2);

        }

    }
    [PunRPC]
    public void GetSFXVolumeRPC(int sound)
    {
        SoundManager.PlayerPlaySoundOneShot(SoundType.WEAPON, sound, 0.4f);
    }
    public void RecoilShake()
    {
        if (pw.IsMine)
        {
            recoilShake.GenerateImpulse();

        }

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
    public Rifle(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime
        , string bullet, Transform firePointnew, ParticleSystem[] particleEffect, Transform raycastDestination)
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
        base.currentAmmo = maxAmmo;
        base.weaponDamage = 20;
        base.firePointnew = firePointnew;
        base.raycastDestination = raycastDestination;
        clipsAmmo = 4;
        base.totalAmmo = clipsAmmo * maxAmmo;


    }
    public override void GetPhotonView(PhotonView pw)
    {
        if (PlayerManager.Instance.pw.IsMine)
        {
            base.pw = pw;
        }
    }
    public override void Reload()
    {
        if (currentAmmo >= 0 && currentAmmo < maxAmmo && totalAmmo > 0)
        {
            totalAmmo += currentAmmo - maxAmmo;
            currentAmmo = maxAmmo;
            base.clipsAmmo--;
            base.returnWarningIndex = 0;
            

        }
        else if (totalAmmo <= 0)
        {
            totalAmmo= 0;
            //Hata Indexini 1'e ayarliyoruz ki burada clipsin bittigini gorelim.
            base.returnWarningIndex = 1;

        }
    }
    public override void AddBullet()
    {
        totalAmmo += maxAmmo;
    }
    public override void Shoot()
    {
        int pelletCount = 1;
        float spreadAngle = 1f;
        currentAmmo--;
        totalAmmo--;

        for (int i = 0; i < pelletCount; i++)
        {
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            Quaternion randomRotation = Quaternion.Euler(firePointnew.rotation.eulerAngles + new Vector3(randomX, randomY, 0));

            Ray ray = new Ray();
            RaycastHit hit;

            ray.origin = firePointnew.position;
            ray.direction = raycastDestination.position - firePointnew.position;
            if (Physics.Raycast(ray, out hit))
            {

                GameObject tracers = PhotonNetwork.Instantiate(Path.Combine("bullet", bullet), firePointnew.position, Quaternion.LookRotation(hit.normal));
                Rigidbody rb = tracers.GetComponent<Rigidbody>();
                rb.AddForce(randomRotation * Vector3.forward * 30f, ForceMode.Impulse);

                if (hit.collider.gameObject.layer == 7 || hit.collider.gameObject.layer == 6)
                {
                    layerAndTagIndex = 0;
                    hitpoint = hit.point;
                    hitnormal = hit.normal;
                }

                if (hit.collider.CompareTag("Playerr"))
                {
                    if (!hit.collider.GetComponent<PhotonView>().IsMine)
                    {
                        hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage, base.pw.OwnerActorNr);
                        layerAndTagIndex = 1;
                        hitpoint = hit.point;
                        hitnormal = hit.normal;
                    }
                }
                if (hit.collider.CompareTag("PlayerBody"))
                {
                    layerAndTagIndex = 2;
                    hitpoint = hit.point;
                    hitnormal = hit.normal;
                }
            }
        }
    }
   
}
class Sniper : WeaponBase
{
    public Sniper(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime
        , string bullet, Transform firePointnew, ParticleSystem[] particleEffect, Transform raycastDestination)
    {
        base.particleEffect = particleEffect;
        base.weaponName = weaponName;
        base.weaponType = WeaponType.SNIPER;
        base.maxAmmo = ammo;
        base.fireRate = firerate;
        base.recoil = recoil;
        base.isAutomatic = isAutomatic;
        base.reloadTime = reloadTime;
        base.bullet = bullet;
        base.currentAmmo = maxAmmo;
        base.weaponDamage = 20;
        base.firePointnew = firePointnew;
        base.raycastDestination = raycastDestination;
        clipsAmmo = 4;
        base.totalAmmo = clipsAmmo * maxAmmo;

    }
    public override void GetPhotonView(PhotonView pw)
    {
        if (PlayerManager.Instance.pw.IsMine)
        {
            base.pw = pw;
        }
    }
    public override void Reload()
    {
        if (currentAmmo >= 0 && currentAmmo < maxAmmo && totalAmmo > 0)
        {
            totalAmmo += currentAmmo - maxAmmo;
            currentAmmo = maxAmmo;
            base.clipsAmmo--;
            base.returnWarningIndex = 0;


        }
        else if (totalAmmo <= 0)
        {
            totalAmmo = 0;
            //Hata Indexini 1'e ayarliyoruz ki burada clipsin bittigini gorelim.
            base.returnWarningIndex = 1;

        }
    }
    public override void AddBullet()
    {
        totalAmmo += maxAmmo;
    }
    public override void Shoot()
    {
        int pelletCount = 1;
        float spreadAngle = 1f;
        currentAmmo--;
        
        for (int i = 0; i < pelletCount; i++)
        {
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            Quaternion randomRotation = Quaternion.Euler(firePointnew.rotation.eulerAngles + new Vector3(randomX, randomY, 0));

            Ray ray = new Ray();
            RaycastHit hit;
            
            ray.origin = firePointnew.position;
            ray.direction = raycastDestination.position - firePointnew.position;
            if (Physics.Raycast(ray, out hit))
            {

                GameObject tracers = PhotonNetwork.Instantiate(Path.Combine("bullet", bullet), firePointnew.position, Quaternion.LookRotation(hit.normal));
                Rigidbody rb = tracers.GetComponent<Rigidbody>();
                rb.AddForce(randomRotation * Vector3.forward * 30f, ForceMode.Impulse);

                if (hit.collider.gameObject.layer == 7 || hit.collider.gameObject.layer == 6)
                {
                    layerAndTagIndex = 0;
                    hitpoint = hit.point;
                    hitnormal = hit.normal;
                }

                if (hit.collider.CompareTag("Playerr"))
                {
                    if (!hit.collider.GetComponent<PhotonView>().IsMine)
                    {
                        hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage, base.pw.OwnerActorNr);
                        layerAndTagIndex = 1;
                        hitpoint = hit.point;
                        hitnormal = hit.normal;
                    }
                }
                if (hit.collider.CompareTag("PlayerBody"))
                {
                    layerAndTagIndex = 2;
                    hitpoint = hit.point;
                    hitnormal = hit.normal;
                }
            }
        }
    }

}
class Smg : WeaponBase
{
    public Smg(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime
        , string bullet, Transform firePointnew, ParticleSystem[] particleEffect, Transform raycastDestination)
    {
        base.particleEffect = particleEffect;
        base.weaponName = weaponName;
        base.weaponType = WeaponType.SMG;
        base.maxAmmo = ammo;
        base.fireRate = firerate;
        base.recoil = recoil;
        base.isAutomatic = isAutomatic;
        base.reloadTime = reloadTime;
        base.bullet = bullet;
        base.currentAmmo = maxAmmo;
        base.weaponDamage = 20;
        base.firePointnew = firePointnew;
        base.raycastDestination = raycastDestination;
        clipsAmmo = 4;
        base.totalAmmo = clipsAmmo * maxAmmo;

    }
    public override void GetPhotonView(PhotonView pw)
    {
        if (PlayerManager.Instance.pw.IsMine)
        {
            base.pw = pw;
        }
    }
    public override void Reload()
    {
        if (currentAmmo >= 0 && currentAmmo < maxAmmo && totalAmmo > 0)
        {
            totalAmmo += currentAmmo - maxAmmo;
            currentAmmo = maxAmmo;
            base.clipsAmmo--;
            base.returnWarningIndex = 0;


        }
        else if (totalAmmo <= 0)
        {
            totalAmmo = 0;
            //Hata Indexini 1'e ayarliyoruz ki burada clipsin bittigini gorelim.
            base.returnWarningIndex = 1;

        }
    }
    public override void AddBullet()
    {
        totalAmmo += maxAmmo;
    }
    public override void Shoot()
    {
        int pelletCount = 1;
        float spreadAngle = 1f;
        currentAmmo--;

        for (int i = 0; i < pelletCount; i++)
        {
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            Quaternion randomRotation = Quaternion.Euler(firePointnew.rotation.eulerAngles + new Vector3(randomX, randomY, 0));

            Ray ray = new Ray();
            RaycastHit hit;

            ray.origin = firePointnew.position;
            ray.direction = raycastDestination.position - firePointnew.position;
            if (Physics.Raycast(ray, out hit))
            {

                GameObject tracers = PhotonNetwork.Instantiate(Path.Combine("bullet", bullet), firePointnew.position, Quaternion.LookRotation(hit.normal));
                Rigidbody rb = tracers.GetComponent<Rigidbody>();
                rb.AddForce(randomRotation * Vector3.forward * 30f, ForceMode.Impulse);

                if (hit.collider.gameObject.layer == 7 || hit.collider.gameObject.layer == 6)
                {
                    layerAndTagIndex = 0;
                    hitpoint = hit.point;
                    hitnormal = hit.normal;
                }

                if (hit.collider.CompareTag("Playerr"))
                {
                    if (!hit.collider.GetComponent<PhotonView>().IsMine)
                    {
                        hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage, base.pw.OwnerActorNr);
                        layerAndTagIndex = 1;
                        hitpoint = hit.point;
                        hitnormal = hit.normal;
                    }
                }
                if (hit.collider.CompareTag("PlayerBody"))
                {
                    layerAndTagIndex = 2;
                    hitpoint = hit.point;
                    hitnormal = hit.normal;
                }
            }
        }

    }
    public IEnumerator DestroySelfBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(0.4f);
        PhotonNetwork.Destroy(bullet);

    }
}

class Pistol : WeaponBase
{
    public Pistol(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime
        , string bullet, Transform firePointnew, ParticleSystem[] particleEffect, Transform raycastDestination)
    {
        base.particleEffect = particleEffect;
        base.weaponName = weaponName;
        base.weaponType = WeaponType.PISTOL;
        base.maxAmmo = ammo;
        base.fireRate = firerate;
        base.recoil = recoil;
        base.isAutomatic = isAutomatic;
        base.reloadTime = reloadTime;
        base.bullet = bullet;
        base.currentAmmo = maxAmmo;
        base.weaponDamage = 20;
        base.firePointnew = firePointnew;
        base.raycastDestination = raycastDestination;
        clipsAmmo = 5;
        base.totalAmmo = clipsAmmo * maxAmmo;

    }
    public override void GetPhotonView(PhotonView pw)
    {
        if (PlayerManager.Instance.pw.IsMine)
        {
            base.pw = pw;
        }
    }
    public override void Reload()
    {
        if (currentAmmo >= 0 && currentAmmo < maxAmmo && totalAmmo > 0)
        {
            totalAmmo += currentAmmo - maxAmmo;
            currentAmmo = maxAmmo;
            base.clipsAmmo--;
            base.returnWarningIndex = 0;


        }
        else if (totalAmmo <= 0)
        {
            totalAmmo = 0;
            //Hata Indexini 1'e ayarliyoruz ki burada clipsin bittigini gorelim.
            base.returnWarningIndex = 1;

        }
    }
    public override void AddBullet()
    {
        totalAmmo += maxAmmo;
    }

    public override void Shoot()
    {
        int pelletCount = 1;
        float spreadAngle = 1f;
        currentAmmo--;

        for (int i = 0; i < pelletCount; i++)
        {
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            Quaternion randomRotation = Quaternion.Euler(firePointnew.rotation.eulerAngles + new Vector3(randomX, randomY, 0));

            Ray ray = new Ray();
            RaycastHit hit;

            ray.origin = firePointnew.position;
            ray.direction = raycastDestination.position - firePointnew.position;
            if (Physics.Raycast(ray, out hit))
            {

                GameObject tracers = PhotonNetwork.Instantiate(Path.Combine("bullet", bullet), firePointnew.position, Quaternion.LookRotation(hit.normal));
                Rigidbody rb = tracers.GetComponent<Rigidbody>();
                rb.AddForce(randomRotation * Vector3.forward * 30f, ForceMode.Impulse);

                if (hit.collider.gameObject.layer == 7 || hit.collider.gameObject.layer == 6)
                {
                    layerAndTagIndex = 0;
                    hitpoint = hit.point;
                    hitnormal = hit.normal;
                }

                if (hit.collider.CompareTag("Playerr"))
                {
                    if (!hit.collider.GetComponent<PhotonView>().IsMine)
                    {
                        hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage, base.pw.OwnerActorNr);
                        layerAndTagIndex = 1;
                        hitpoint = hit.point;
                        hitnormal = hit.normal;
                    }
                }
                if (hit.collider.CompareTag("PlayerBody"))
                {
                    layerAndTagIndex = 2;
                    hitpoint = hit.point;
                    hitnormal = hit.normal;
                }
            }
        }

    }

    public IEnumerator DestroySelfBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(0.4f);
        PhotonNetwork.Destroy(bullet);

    }
}
class Shotgun : WeaponBase
{
    public Shotgun(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime
        , string bullet, Transform firePointnew, ParticleSystem[] particleEffect, Transform raycastDestination)
    {
        base.particleEffect = particleEffect;
        base.weaponName = weaponName;
        base.weaponType = WeaponType.SHOTGUN;
        base.maxAmmo = ammo;
        base.fireRate = firerate;
        base.recoil = recoil;
        base.isAutomatic = isAutomatic;
        base.reloadTime = reloadTime;
        base.bullet = bullet;
        base.currentAmmo = maxAmmo;
        base.weaponDamage = 20;
        base.firePointnew = firePointnew;
        base.raycastDestination = raycastDestination;
        clipsAmmo = 5;
        base.totalAmmo = clipsAmmo * maxAmmo;
    }
    public override void GetPhotonView(PhotonView pw)
    {
        if (PlayerManager.Instance.pw.IsMine)
        {
            base.pw = pw;
        }
    }
    public override void Reload()
    {
        if (currentAmmo >= 0 && currentAmmo < maxAmmo && totalAmmo > 0)
        {
            totalAmmo += currentAmmo - maxAmmo;
            currentAmmo = maxAmmo;
            base.clipsAmmo--;
            base.returnWarningIndex = 0;


        }
        else if (totalAmmo <= 0)
        {
            totalAmmo = 0;
            //Hata Indexini 1'e ayarliyoruz ki burada clipsin bittigini gorelim.
            base.returnWarningIndex = 1;

        }
    }
    public override void AddBullet()
    {
        totalAmmo += maxAmmo;
    }

    public override void Shoot()
    {
        int pelletCount = 8; // Pompalı için birden fazla saçma
        float spreadAngle = 1f; // Saçmaların yayılma açısı
        float maxRange = 50f; // Maksimum menzil
        currentAmmo--;

        for (int i = 0; i < pelletCount; i++)
        {
            // Saçma yayılma açısını rastgele oluştur
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            Quaternion randomRotation = Quaternion.Euler(firePointnew.rotation.eulerAngles + new Vector3(randomX, randomY, 0));

            Ray ray = new Ray();
            RaycastHit hit;

            ray.origin = firePointnew.position;
            ray.direction = randomRotation * (raycastDestination.position - firePointnew.position).normalized;


            if (Physics.Raycast(ray, out hit, maxRange))
            {
                Debug.DrawLine(firePointnew.position, hit.point, Color.green, 1f); 


                // Merminin çarpma noktası ve kuvveti
                GameObject tracers = PhotonNetwork.Instantiate(Path.Combine("bullet", bullet), firePointnew.position, Quaternion.LookRotation(hit.normal));
                Rigidbody rb = tracers.GetComponent<Rigidbody>();
                rb.AddForce(randomRotation * Vector3.forward * 30f, ForceMode.Impulse);

                if (hit.collider.gameObject.layer == 7 || hit.collider.gameObject.layer == 6)
                {
                    layerAndTagIndex = 0;
                    hitpoint = hit.point;
                    hitnormal = hit.normal;
                }

                if (hit.collider.CompareTag("Playerr"))
                {
                    if (!hit.collider.GetComponent<PhotonView>().IsMine)
                    {
                        hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage,base.pw.OwnerActorNr);
                        layerAndTagIndex = 1;
                        hitpoint = hit.point;
                        hitnormal = hit.normal;
                    }
                }

                if (hit.collider.CompareTag("PlayerBody"))
                {
                    layerAndTagIndex = 2;
                    hitpoint = hit.point;
                    hitnormal = hit.normal;
                }
            }
        }
    }
    public IEnumerator DestroySelfBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(0.4f);
        PhotonNetwork.Destroy(bullet);

    }
}
