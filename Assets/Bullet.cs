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

    private void OnEnable()
    {
        StartCoroutine(DestroyGameObject());
    }
    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    

}
