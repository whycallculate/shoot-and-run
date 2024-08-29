using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Transactions;
public enum BulletTpye
{
    EMPTY,
    RIFLE_AMMO,
    SMG_AMMO,
    SHOTGUN_AMMO,
    PISTOL_AMMO
}
public class Bullet : MonoBehaviour
{
    PhotonView pw;
    [SerializeField] BulletTpye bulletTpye;

    private void Awake()
    {
        pw = GetComponent<PhotonView>();
        StartCoroutine(DestroyGameObject());
    }
    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "PlayerBody" || other.tag == "PlayerHead")
        {
            if (pw.IsMine)
            {
                // Eğer mermi senin oyuncuna aitse, hasar verme işlemini başlat.
                StartCoroutine(GiveDamage(other));

            }
        }
    }

    public IEnumerator GiveDamage(Collider other)
    {
        int damage = 0;

        if (bulletTpye == BulletTpye.RIFLE_AMMO)
            damage = other.tag == "PlayerHead" ? 16 : 23;
        else if (bulletTpye == BulletTpye.PISTOL_AMMO)
            damage = other.tag == "PlayerHead" ? 12 : 19;
        else if (bulletTpye == BulletTpye.SHOTGUN_AMMO)
            damage = other.tag == "PlayerHead" ? 8 : 10;
        else if (bulletTpye == BulletTpye.SMG_AMMO)
            damage = other.tag == "PlayerHead" ? 12 : 15;
        other.transform.GetParentComponent<PlayerManager>().TakeDamage(damage);

        
        yield return new WaitForSeconds(0.2f);
    }



    public void HitDiffrentObject() { }
}
