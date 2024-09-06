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
    [SerializeField] WeaponType type;
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
    WeaponBase weapon;
    public PhotonView pw;
    public bool notShooting = false;

    private void Start()
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
            if (Input.GetKeyDown(KeyCode.R))
            {

                StartCoroutine(ReloadOnGame());
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
            Debug.Log("1.");
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
            Debug.Log("2.");
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
            if(weapon.weaponType == WeaponType.SHOTGUN)
            {
                pw.RPC("GetSFXVolumeRPC", RpcTarget.All,3);
            }
            else if(weapon.weaponType == WeaponType.RIFLE)
            {
                pw.RPC("GetSFXVolumeRPC", RpcTarget.All, 0);

            }
            else if(weapon.weaponType == WeaponType.SMG)
            {
                pw.RPC("GetSFXVolumeRPC", RpcTarget.All, 4);

            }
            else if(weapon.weaponType == WeaponType.PISTOL)
            {
                pw.RPC("GetSFXVolumeRPC", RpcTarget.All, 1);

            }
            else if(weapon.weaponType == WeaponType.SNIPER)
            {
                pw.RPC("GetSFXVolumeRPC", RpcTarget.All, 2);

            }
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
    IEnumerator ReloadOnGame()
    {
        if (weapon.currentAmmo != weapon.maxAmmo)
        {
            
            state = ShootState.RELOADING;
            yield return new WaitForSeconds(1f);
            weapon.Reload();
            

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
                            state = ShootState.SHOOTING;
                            notShooting = true;
                            yield return new WaitForSeconds(weapon.fireRate);
                            weapon.Shoot();
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
                        if (weapon.currentAmmo == weapon.maxAmmo || weapon.currentAmmo > 0)
                        {
                            GenerateRecoil();
                            state = ShootState.SHOOTING;
                            notShooting = true;
                            yield return new WaitForSeconds(weapon.fireRate);
                            weapon.Shoot();
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
                            state = ShootState.SHOOTING;
                            notShooting = true;
                            weapon.Shoot();
                            notShooting = false;

                        }
                        else if (weapon.currentAmmo <= 0)
                        {
                            state = ShootState.RELOADING;
                            yield return new WaitForSeconds(1f);
                            weapon.Reload();
                        }
                        yield return new WaitForSeconds(weapon.fireRate);

                    }
                }
                else if (weapon.isAutomatic)
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        if (weapon.currentAmmo == weapon.maxAmmo || weapon.currentAmmo > 0)
                        {
                            GenerateRecoil();
                            state = ShootState.SHOOTING;
                            notShooting = true;
                            weapon.Shoot();
                            notShooting = false;
                        }
                        else if (weapon.currentAmmo <= 0)
                        {
                            state = ShootState.RELOADING;
                            yield return new WaitForSeconds(1f);
                            weapon.Reload();
                        }
                        yield return new WaitForSeconds(weapon.fireRate);

                    }
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
    public Rifle(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePointnew, ParticleSystem[] particleEffect, Transform raycastDestination)
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

                particleEffect[0].Emit(1);
                particleEffect[1].Emit(1);
                Debug.DrawLine(ray.origin, hit.point, Color.red, 4f);

                particleEffect[2].transform.position = hit.point;
                particleEffect[2].transform.forward = hit.normal;
                particleEffect[2].Emit(1);

                particleEffect[3].transform.position = hit.point;
                particleEffect[3].transform.forward = hit.normal;
                particleEffect[3].Emit(1);

                particleEffect[4].transform.position = hit.point;
                particleEffect[4].transform.forward = hit.normal;
                particleEffect[4].Emit(1);

                particleEffect[5].transform.position = hit.point;
                particleEffect[5].transform.forward = hit.normal;
                particleEffect[5].Emit(1);

                if (hit.collider.CompareTag("Playerr"))
                {
                    if (!hit.collider.GetComponent<PhotonView>().IsMine)
                    {
                        hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage);
                        particleEffect[6].Emit(1);
                        particleEffect[7].Emit(1);
                        particleEffect[8].Emit(1);
                    }

                }

                {

                }



            }
        }
    }


}
class Sniper : WeaponBase
{
    public Sniper(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePointnew, ParticleSystem[] particleEffect, Transform raycastDestination)
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

                particleEffect[0].Emit(1);
                particleEffect[1].Emit(1);
                Debug.DrawLine(ray.origin, hit.point, Color.red, 4f);

                particleEffect[2].transform.position = hit.point;
                particleEffect[2].transform.forward = hit.normal;
                particleEffect[2].Emit(1);

                particleEffect[3].transform.position = hit.point;
                particleEffect[3].transform.forward = hit.normal;
                particleEffect[3].Emit(1);

                particleEffect[4].transform.position = hit.point;
                particleEffect[4].transform.forward = hit.normal;
                particleEffect[4].Emit(1);

                particleEffect[5].transform.position = hit.point;
                particleEffect[5].transform.forward = hit.normal;
                particleEffect[5].Emit(1);

                if (hit.collider.CompareTag("Playerr"))
                {
                    if (!hit.collider.GetComponent<PhotonView>().IsMine)
                    {
                        hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage);
                        particleEffect[6].Emit(1);
                        particleEffect[7].Emit(1);
                        particleEffect[8].Emit(1);
                    }
                }

                {

                }



            }
        }
    }

}
class Smg : WeaponBase
{
    public Smg(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePointnew, ParticleSystem[] particleEffect, Transform raycastDestination)
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
    }

    public override void Reload()
    {

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

                particleEffect[0].Emit(1);
                particleEffect[1].Emit(1);
                Debug.DrawLine(ray.origin, hit.point, Color.red, 4f);

                particleEffect[2].transform.position = hit.point;
                particleEffect[2].transform.forward = hit.normal;
                particleEffect[2].Emit(1);

                particleEffect[3].transform.position = hit.point;
                particleEffect[3].transform.forward = hit.normal;
                particleEffect[3].Emit(1);

                particleEffect[4].transform.position = hit.point;
                particleEffect[4].transform.forward = hit.normal;
                particleEffect[4].Emit(1);

                particleEffect[5].transform.position = hit.point;
                particleEffect[5].transform.forward = hit.normal;
                particleEffect[5].Emit(1);

                if (hit.collider.CompareTag("Playerr"))
                {
                    if (!hit.collider.GetComponent<PhotonView>().IsMine)
                    {
                        hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage);
                        particleEffect[6].Emit(1);
                        particleEffect[7].Emit(1);
                        particleEffect[8].Emit(1);
                    }
                }

                {

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
    public Pistol(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePointnew, ParticleSystem[] particleEffect, Transform raycastDestination)
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
    }

    public override void Reload()
    {

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

                particleEffect[0].Emit(1);
                particleEffect[1].Emit(1);
                Debug.DrawLine(ray.origin, hit.point, Color.red, 4f);

                particleEffect[2].transform.position = hit.point;
                particleEffect[2].transform.forward = hit.normal;
                particleEffect[2].Emit(1);

                particleEffect[3].transform.position = hit.point;
                particleEffect[3].transform.forward = hit.normal;
                particleEffect[3].Emit(1);

                particleEffect[4].transform.position = hit.point;
                particleEffect[4].transform.forward = hit.normal;
                particleEffect[4].Emit(1);

                particleEffect[5].transform.position = hit.point;
                particleEffect[5].transform.forward = hit.normal;
                particleEffect[5].Emit(1);

                if (hit.collider.CompareTag("Playerr"))
                {
                    if (!hit.collider.GetComponent<PhotonView>().IsMine)
                    {
                        hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage);
                        particleEffect[6].Emit(1);
                        particleEffect[7].Emit(1);
                        particleEffect[8].Emit(1);
                    }
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
    public Shotgun(string weaponName, int ammo, float firerate, float recoil, bool isAutomatic, float reloadTime, string bullet, Transform firePointnew, ParticleSystem[] particleEffect, Transform raycastDestination)
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
    }

    public override void Reload()
    {
        currentAmmo = maxAmmo;
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

                particleEffect[0].Emit(1);
                particleEffect[1].Emit(1);
                Debug.DrawLine(ray.origin, hit.point, Color.red, 4f);

                particleEffect[2].transform.position = hit.point;
                particleEffect[2].transform.forward = hit.normal;
                particleEffect[2].Emit(1);

                particleEffect[3].transform.position = hit.point;
                particleEffect[3].transform.forward = hit.normal;
                particleEffect[3].Emit(1);

                particleEffect[4].transform.position = hit.point;
                particleEffect[4].transform.forward = hit.normal;
                particleEffect[4].Emit(1);

                particleEffect[5].transform.position = hit.point;
                particleEffect[5].transform.forward = hit.normal;
                particleEffect[5].Emit(1);

                if (hit.collider.CompareTag("Playerr"))
                {
                    if (!hit.collider.GetComponent<PhotonView>().IsMine)
                    {
                        hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage);
                        particleEffect[6].Emit(1);
                        particleEffect[7].Emit(1);
                        particleEffect[8].Emit(1);
                    }
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
