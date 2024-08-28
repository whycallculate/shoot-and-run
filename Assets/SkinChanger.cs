using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    int i = 0;
    int getIndex;
    int setIndex;

    
    private void Awake()
    {
        getIndex = 1;
        pw = gameObject.GetComponent<PhotonView>();

    }
    private void Start()
    {
        SkinChangeMethod();
    }
    private void Update()
    {


        //if
        //{
        //    Debug.Log(setIndex + getIndex + i);

        //}

    }

    public void SkinChangeMethod()
    {
        if (pw.IsMine)
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
                setIndex = playerModelObject[i + 1].GetComponent<ModelGetData>().costumIndex;
                playerModelObject[i].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
                playerModelObject[i].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
                playerModelObject[i].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
                playerModelObject[i].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);
                
                i++;
            }
            if (i == playerModelObject.Length - 1)
            {
                i = 0;
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);
                SkinChangeMethod();
            }                                                
            AimState.Instance.rig.Clear();
            AimState.Instance.rig.Build();
            AimState.Instance.anim.Rebind();
            AimState.Instance.anim.Update(0f);
        }

    }
    public void OtherPlayerSetSkin(int getIndex)
    {
        if(!pw.IsMine)
        {

            if(getIndex <= 1)
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
            else if(getIndex > 1 && getIndex < playerModelObject.Length -1)
            {
                playerModelObject[getIndex].GetComponent<ModelGetData>().Body.gameObject.SetActive(true);
                playerModelObject[getIndex].GetComponent<ModelGetData>().Feet.gameObject.SetActive(true);
                playerModelObject[getIndex].GetComponent<ModelGetData>().Head.gameObject.SetActive(true);
                playerModelObject[getIndex].GetComponent<ModelGetData>().Legs.gameObject.SetActive(true);
                playerModelObject[getIndex].GetComponent<ModelGetData>().Body.bones = playerModelObject[getIndex -1].GetComponent<ModelGetData>().Body.bones;
                playerModelObject[getIndex].GetComponent<ModelGetData>().Feet.bones = playerModelObject[getIndex -1].GetComponent<ModelGetData>().Feet.bones;
                playerModelObject[getIndex].GetComponent<ModelGetData>().Head.bones = playerModelObject[getIndex -1].GetComponent<ModelGetData>().Head.bones;
                playerModelObject[getIndex].GetComponent<ModelGetData>().Legs.bones = playerModelObject[getIndex -1].GetComponent<ModelGetData>().Legs.bones;
                playerModelObject[getIndex].GetComponent<ModelGetData>().Body.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[getIndex].GetComponent<ModelGetData>().Feet.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[getIndex].GetComponent<ModelGetData>().Head.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[getIndex].GetComponent<ModelGetData>().Legs.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[getIndex - 1].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
                playerModelObject[getIndex - 1].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
                playerModelObject[getIndex - 1].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
                playerModelObject[getIndex - 1].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);
            }
            else if(getIndex == playerModelObject.Length - 1)
            {
                this.getIndex = 0;
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
                playerModelObject[playerModelObject.Length - 1].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);
                OtherPlayerSetSkin(this.getIndex);
            }

            AimState.Instance.rig.Clear();
            AimState.Instance.rig.Build();
            AimState.Instance.anim.Rebind();
            AimState.Instance.anim.Update(0f);
        }
    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(setIndex);
        }
        else
        {
            if (!pw.IsMine)
            {
                getIndex = (int)stream.ReceiveNext();
                OtherPlayerSetSkin(getIndex);
            }

        }

    }
}
