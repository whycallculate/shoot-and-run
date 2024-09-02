using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Profiling;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] Transform crossHairTarget;
    [SerializeField] Animator playerAnim;
    [SerializeField] public Transform weaponLeftGrip;
    [SerializeField] public Transform weaponRightGrip;
    public Transform weaponParent;
    public Rig handIK;

    Weapons weapon;
    Animator anim;
    AnimatorOverrideController overrideAnim;


    public PhotonView pw;
    // Start is called before the first frame update
    void Start()
    {
        pw = GetComponent<PhotonView>();
        if (pw.IsMine)
        {
            anim = GetComponent<Animator>();
            overrideAnim = anim.runtimeAnimatorController as AnimatorOverrideController;
            Weapons newWeapons = GetComponentInChildren<Weapons>();
            if (newWeapons)
            {
                Equip(newWeapons);
            }
        }


    }
    private void Update()
    {
        if (!weapon)
        {
            handIK.weight = 0f;
        }
    }

    public void Equip(GameObject gameObjectWeapon)
    {
        if (weapon)
        {
            PhotonNetwork.Destroy(weapon.gameObject);
        }
        else
        {
            handIK.weight = 0f;
            anim.SetLayerWeight(1, 0f);
        }
        weapon = gameObjectWeapon.GetComponent<Weapons>();
        weapon.raycastDestination = crossHairTarget;
        weapon.anim = playerAnim;
        handIK.weight = 1f;

        anim.SetLayerWeight(1, 1.0f);
        Invoke(nameof(SetAnimationDelayed), 0.001f);
    }
    public void Equip(Weapons weapon)
    {

        this.weapon = weapon;
        this.weapon.raycastDestination = crossHairTarget;
        this.weapon.anim = playerAnim;
        handIK.weight = 1f;

        anim.SetLayerWeight(1, 1.0f);
        Invoke(nameof(SetAnimationDelayed), 0.001f);
    }

    void SetAnimationDelayed()
    {
        overrideAnim["weaponAnimEmpty"] = weapon.weaponAnimation;

    }
    [ContextMenu("Save weapon pose")]
    void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(weaponParent.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponLeftGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponRightGrip.gameObject, false);
        recorder.TakeSnapshot(0.0f);
        recorder.SaveToClip(weapon.weaponAnimation);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}
