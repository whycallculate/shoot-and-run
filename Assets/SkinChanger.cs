using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SkinChanger : MonoBehaviour
{
    private static SkinChanger instance;
    public static SkinChanger Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SkinChanger>();
            }
            return instance;
        }
    }
    PhotonView pw;
    [SerializeField] public GameObject[] playerModelObject;
    public Avatar getAvatar;
    public Rig getModelRig;
    public MultiAimConstraint getModelBodyAim;
    public MultiAimConstraint getModelHeadAim;
    public MultiAimConstraint getModelRHandAim;
    public TwoBoneIKConstraint getModelLHandIK;
    public GameObject[] getWeaponList;
    int i = 0;


    private void Awake()
    {
        pw = GetComponent<PhotonView>();
        if (pw.IsMine)
        {
            GetSkinDataMethod(i);

        }
    }

    public void SkinChangeMethod()
    {
        
        if(i >= 0 && i < playerModelObject.Length -1)
        {
            playerModelObject[i+1].SetActive(true);


            playerModelObject[i+1].GetComponent<ModelGetData>().Body.bones = playerModelObject[i].GetComponent<ModelGetData>().Body.bones;
            playerModelObject[i+1].GetComponent<ModelGetData>().Feet.bones = playerModelObject[i].GetComponent<ModelGetData>().Feet.bones;
            playerModelObject[i+1].GetComponent<ModelGetData>().Head.bones = playerModelObject[i].GetComponent<ModelGetData>().Head.bones;
            playerModelObject[i+1].GetComponent<ModelGetData>().Legs.bones = playerModelObject[i].GetComponent<ModelGetData>().Legs.bones;
            playerModelObject[i + 1].GetComponent<ModelGetData>().Body.rootBone = playerModelObject[i + 1].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i + 1].GetComponent<ModelGetData>().Feet.rootBone = playerModelObject[i + 1].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i + 1].GetComponent<ModelGetData>().Head.rootBone = playerModelObject[i + 1].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i + 1].GetComponent<ModelGetData>().Legs.rootBone = playerModelObject[i + 1].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i].SetActive(false);

            i++;
            
            GetSkinDataMethod(i);
        }
        if(i > playerModelObject.Length - 1)
        {
            i = 0;
        }
    }



    public void GetSkinDataMethod(int b)
    {
        playerModelObject[b].GetComponent<ModelGetData>();
        getAvatar = playerModelObject[b].GetComponent<ModelGetData>().avatar;
        getModelRig = playerModelObject[b].GetComponent<ModelGetData>().modelRig;
        getModelBodyAim = playerModelObject[b].GetComponent<ModelGetData>().modelBodyAim;
        getModelHeadAim = playerModelObject[b].GetComponent<ModelGetData>().modelHeadAim;
        getModelRHandAim = playerModelObject[b].GetComponent<ModelGetData>().modelRHandAim;
        getModelLHandIK = playerModelObject[b].GetComponent<ModelGetData>().modelLHandIK;
        getWeaponList = playerModelObject[b].GetComponent<ModelGetData>().weaponList;
    }
}
