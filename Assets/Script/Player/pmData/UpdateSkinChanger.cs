using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSkinChanger : MonoBehaviour
{
    [SerializeField] public GameObject[] playerModelObject;

    int i = 0;
    public void NextChangeSkinUI()
    {
        if (i >= 0 && i < playerModelObject.Length - 1)
        {
            i++;
            playerModelObject[i].SetActive(true);
            playerModelObject[0].GetComponent<ModelGetData>().Backpack.gameObject.SetActive(true);
            playerModelObject[0].GetComponent<ModelGetData>().Backpack.bones = playerModelObject[0].GetComponent<ModelGetData>().Body.bones;
            playerModelObject[0].GetComponent<ModelGetData>().Backpack.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i].GetComponent<ModelGetData>().Body.gameObject.SetActive(true);
            playerModelObject[i].GetComponent<ModelGetData>().Feet.gameObject.SetActive(true);
            playerModelObject[i].GetComponent<ModelGetData>().Head.gameObject.SetActive(true);
            playerModelObject[i].GetComponent<ModelGetData>().Legs.gameObject.SetActive(true);
            playerModelObject[i].GetComponent<ModelGetData>().Body.bones = playerModelObject[i -1].GetComponent<ModelGetData>().Body.bones;
            playerModelObject[i].GetComponent<ModelGetData>().Feet.bones = playerModelObject[i -1].GetComponent<ModelGetData>().Feet.bones;
            playerModelObject[i].GetComponent<ModelGetData>().Head.bones = playerModelObject[i -1].GetComponent<ModelGetData>().Head.bones;
            playerModelObject[i].GetComponent<ModelGetData>().Legs.bones = playerModelObject[i -1].GetComponent<ModelGetData>().Legs.bones;
            playerModelObject[i].GetComponent<ModelGetData>().Body.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i].GetComponent<ModelGetData>().Feet.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i].GetComponent<ModelGetData>().Head.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i].GetComponent<ModelGetData>().Legs.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i -1].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
            playerModelObject[i -1].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
            playerModelObject[i -1].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
            playerModelObject[i -1].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);

            
        }
        else
        {
            return;
        }

    }
    public void PreviousChangeSkinUI()
    {
        if(i <= playerModelObject.Length - 1 && i >= 1) 
        {
            i--;
            playerModelObject[i].SetActive(true);
            playerModelObject[0].GetComponent<ModelGetData>().Backpack.gameObject.SetActive(true);
            playerModelObject[0].GetComponent<ModelGetData>().Backpack.bones = playerModelObject[0].GetComponent<ModelGetData>().Body.bones;
            playerModelObject[0].GetComponent<ModelGetData>().Backpack.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i].GetComponent<ModelGetData>().Body.gameObject.SetActive(true);
            playerModelObject[i].GetComponent<ModelGetData>().Feet.gameObject.SetActive(true);
            playerModelObject[i].GetComponent<ModelGetData>().Head.gameObject.SetActive(true);
            playerModelObject[i].GetComponent<ModelGetData>().Legs.gameObject.SetActive(true);
            playerModelObject[i].GetComponent<ModelGetData>().Body.bones = playerModelObject[i +1].GetComponent<ModelGetData>().Body.bones;
            playerModelObject[i].GetComponent<ModelGetData>().Feet.bones = playerModelObject[i +1].GetComponent<ModelGetData>().Feet.bones;
            playerModelObject[i].GetComponent<ModelGetData>().Head.bones = playerModelObject[i +1].GetComponent<ModelGetData>().Head.bones;
            playerModelObject[i].GetComponent<ModelGetData>().Legs.bones = playerModelObject[i +1].GetComponent<ModelGetData>().Legs.bones;
            playerModelObject[i].GetComponent<ModelGetData>().Body.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i].GetComponent<ModelGetData>().Feet.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i].GetComponent<ModelGetData>().Head.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i].GetComponent<ModelGetData>().Legs.rootBone = playerModelObject[0].GetComponent<ModelGetData>().modelRoot;
            playerModelObject[i +1].GetComponent<ModelGetData>().Body.gameObject.SetActive(false);
            playerModelObject[i +1].GetComponent<ModelGetData>().Feet.gameObject.SetActive(false);
            playerModelObject[i +1].GetComponent<ModelGetData>().Head.gameObject.SetActive(false);
            playerModelObject[i +1].GetComponent<ModelGetData>().Legs.gameObject.SetActive(false);
        }
        else
        {
            return;
        }

    }

    public void UpdateSkinChange() 
    {
        PlayerData.Instance.UpdateCharacterAppearance(i);
    }

}
