using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SkinChanger : MonoBehaviour, IPunObservable
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
    //public Avatar getAvatar;
    //public Rig getModelRig;
    //public MultiAimConstraint getModelBodyAim;
    //public MultiAimConstraint getModelHeadAim;
    //public MultiAimConstraint getModelRHandAim;
    //public TwoBoneIKConstraint getModelLHandIK;
    //public GameObject[] getWeaponList;
    int i = 0;
    int getIndex;


    private void Awake()
    {
        pw = GetComponent<PhotonView>();
        if (pw.IsMine)
        {
            //GetSkinDataMethod(i);


        }
    }

    public void SkinChangeMethod()
    {
        if(pw.IsMine)
        {

            if (i >= 0 && i < playerModelObject.Length - 1)
            {
                playerModelObject[i + 1].SetActive(true);

                playerModelObject[i + 1].GetComponent<ModelGetData>().Body.gameObject.SetActive(true);
                playerModelObject[i + 1].GetComponent<ModelGetData>().Feet.gameObject.SetActive(true);
                playerModelObject[i + 1].GetComponent<ModelGetData>().Head.gameObject.SetActive(true);
                playerModelObject[i + 1].GetComponent<ModelGetData>().Legs.gameObject.SetActive(true);
                playerModelObject[i + 1].GetComponent<ModelGetData>().Body.bones = playerModelObject[i].GetComponent<ModelGetData>().Body.bones;
                playerModelObject[i + 1].GetComponent<ModelGetData>().Feet.bones = playerModelObject[i].GetComponent<ModelGetData>().Feet.bones;
                playerModelObject[i + 1].GetComponent<ModelGetData>().Head.bones = playerModelObject[i].GetComponent<ModelGetData>().Head.bones;
                playerModelObject[i + 1].GetComponent<ModelGetData>().Legs.bones = playerModelObject[i].GetComponent<ModelGetData>().Legs.bones;
                playerModelObject[i + 1].GetComponent<ModelGetData>().Body.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i + 1].GetComponent<ModelGetData>().Feet.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i + 1].GetComponent<ModelGetData>().Head.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i + 1].GetComponent<ModelGetData>().Legs.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
                playerModelObject[i].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
                playerModelObject[i].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
                playerModelObject[i].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);
                i++;
                //GetSkinDataMethod(i);
            }
            if (i == playerModelObject.Length - 1)
            {
                i = 0;
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);
            }
        }
        
    }
    public void OtherPlayerSetSkin(int getIndex)
    {
        if(!pw.IsMine)
        {
            playerModelObject[getIndex].GetComponent<ModelGetData>().Body.gameObject.SetActive(true);
            playerModelObject[getIndex].GetComponent<ModelGetData>().Feet.gameObject.SetActive(true);
            playerModelObject[getIndex].GetComponent<ModelGetData>().Head.gameObject.SetActive(true);
            playerModelObject[getIndex].GetComponent<ModelGetData>().Legs.gameObject.SetActive(true);
            playerModelObject[getIndex].GetComponent<ModelGetData>().Body.bones = playerModelObject[0].GetComponent<ModelGetData>().Body.bones;
            playerModelObject[getIndex].GetComponent<ModelGetData>().Feet.bones = playerModelObject[0].GetComponent<ModelGetData>().Feet.bones;
            playerModelObject[getIndex].GetComponent<ModelGetData>().Head.bones = playerModelObject[0].GetComponent<ModelGetData>().Head.bones;
            playerModelObject[getIndex].GetComponent<ModelGetData>().Legs.bones = playerModelObject[0].GetComponent<ModelGetData>().Legs.bones;
            playerModelObject[getIndex].GetComponent<ModelGetData>().Body.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[getIndex].GetComponent<ModelGetData>().Feet.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[getIndex].GetComponent<ModelGetData>().Head.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[getIndex].GetComponent<ModelGetData>().Legs.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[0].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
            playerModelObject[0].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
            playerModelObject[0].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
            playerModelObject[0].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);
        }
    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(i);
        }
        else
        {
            OtherPlayerSetSkin((int)stream.ReceiveNext());
        }

    }
}
