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
    int getIndex;
    int setIndex;

    
    private void Awake()
    {
        pw = gameObject.GetComponent<PhotonView>();

    }
    private void Start()
    {

        //Eger TestScenedeysen buradaki degeri 1 verebilirsin Yoksa Bunu vereceksin PlayerData.Instance.playerData.user.costume_index
        SkinChangeMethod(PlayerData.Instance.playerData.user.costume_index);
    }
    public void SkinChangeMethod(int i )
    {
        if (pw.IsMine)
        {
            if(i == 0)
            {
                playerModelObject[i].SetActive(true);
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.gameObject.SetActive(true);
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.bones = playerModelObject[0].GetComponent<ModelGetData>().Body.bones;
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i].GetComponent<ModelGetData>().Body.gameObject.SetActive(true);
                playerModelObject[i].GetComponent<ModelGetData>().Feet.gameObject.SetActive(true);
                playerModelObject[i].GetComponent<ModelGetData>().Head.gameObject.SetActive(true);
                playerModelObject[i].GetComponent<ModelGetData>().Legs.gameObject.SetActive(true);
                playerModelObject[i].GetComponent<ModelGetData>().Body.bones = playerModelObject[0].GetComponent<ModelGetData>().Body.bones;
                playerModelObject[i].GetComponent<ModelGetData>().Feet.bones = playerModelObject[0].GetComponent<ModelGetData>().Feet.bones;
                playerModelObject[i].GetComponent<ModelGetData>().Head.bones = playerModelObject[0].GetComponent<ModelGetData>().Head.bones;
                playerModelObject[i].GetComponent<ModelGetData>().Legs.bones = playerModelObject[0].GetComponent<ModelGetData>().Legs.bones;
                playerModelObject[i].GetComponent<ModelGetData>().Body.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i].GetComponent<ModelGetData>().Feet.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i].GetComponent<ModelGetData>().Head.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i].GetComponent<ModelGetData>().Legs.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                setIndex = playerModelObject[i].GetComponent<ModelGetData>().costumIndex;
            }
            else
            {
                playerModelObject[i].SetActive(true);
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.gameObject.SetActive(true);
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.bones = playerModelObject[0].GetComponent<ModelGetData>().Body.bones;
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i].GetComponent<ModelGetData>().Body.gameObject.SetActive(true);
                playerModelObject[i].GetComponent<ModelGetData>().Feet.gameObject.SetActive(true);
                playerModelObject[i].GetComponent<ModelGetData>().Head.gameObject.SetActive(true);
                playerModelObject[i].GetComponent<ModelGetData>().Legs.gameObject.SetActive(true);
                playerModelObject[i].GetComponent<ModelGetData>().Body.bones = playerModelObject[0].GetComponent<ModelGetData>().Body.bones;
                playerModelObject[i].GetComponent<ModelGetData>().Feet.bones = playerModelObject[0].GetComponent<ModelGetData>().Feet.bones;
                playerModelObject[i].GetComponent<ModelGetData>().Head.bones = playerModelObject[0].GetComponent<ModelGetData>().Head.bones;
                playerModelObject[i].GetComponent<ModelGetData>().Legs.bones = playerModelObject[0].GetComponent<ModelGetData>().Legs.bones;
                playerModelObject[i].GetComponent<ModelGetData>().Body.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i].GetComponent<ModelGetData>().Feet.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i].GetComponent<ModelGetData>().Head.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                playerModelObject[i].GetComponent<ModelGetData>().Legs.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
                setIndex = playerModelObject[i].GetComponent<ModelGetData>().costumIndex;
                playerModelObject[0].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
                playerModelObject[0].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
                playerModelObject[0].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
                playerModelObject[0].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);
            }

        }

    }
    public void OtherPlayerSetSkin(int getIndex)
    {
        if(!pw.IsMine)
        {
            if (getIndex == 0)
            {
                playerModelObject[getIndex].SetActive(true);
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.gameObject.SetActive(true);
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.bones = playerModelObject[0].GetComponent<ModelGetData>().Body.bones;
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
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
                setIndex = playerModelObject[getIndex].GetComponent<ModelGetData>().costumIndex;
            }
            else
            {
                playerModelObject[getIndex].SetActive(true);
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.gameObject.SetActive(true);
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.bones = playerModelObject[0].GetComponent<ModelGetData>().Body.bones;
                playerModelObject[0].GetComponent<ModelGetData>().Backpack.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
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
                setIndex = playerModelObject[getIndex].GetComponent<ModelGetData>().costumIndex;
                playerModelObject[0].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
                playerModelObject[0].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
                playerModelObject[0].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
                playerModelObject[0].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);
            }

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
