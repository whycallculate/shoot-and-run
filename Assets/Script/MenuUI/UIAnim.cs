using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class UIAnim : MonoBehaviour
{

    public void ScaleUpDownLogo(GameObject item, float time, float scale)
    {
        LeanTween.cancel(item);


        item.transform.localScale = Vector3.one;

        LeanTween.scale(item, Vector3.one * scale, time).setEasePunch();
    }

    public void NewSceneChange(GameObject item)
    {
        item.SetActive(true);
        LeanTween.scale(item, Vector3.one, 0);
        LeanTween.scale(item, Vector3.zero, 1f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() =>
        {
            item.SetActive(false);

        });
    }
    public void ChangeScene(GameObject item, int i)
    {
        item.SetActive(true);
        LeanTween.scale(item, Vector3.zero, 0);
        LeanTween.scale(item, Vector3.one, 1f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() =>
        {
            PhotonNetwork.LoadLevel(i);
            
        });
        
    }


    public void OnEnabled()
    {

        //Acildigi zaman Bir anda karsina cikma anim
        gameObject.SetActive(true);
        LeanTween.scale(gameObject.GetComponent<RectTransform>(), new Vector3(1, 1, 1), 0.4f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(openGameObject);
    }
    void openGameObject()
    {
        gameObject.SetActive(true);
    }
    public void OnDisabled()
    {
        //Kapandigi zaman hizlic
        LeanTween.scale(gameObject.GetComponent<RectTransform>(), new Vector3(0, 0, 0), 0.4f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(closeGameObject);
    }
    void closeGameObject()
    {
        gameObject.SetActive(false);
    }

    public void OnEnabledWithUp() 
    {
        gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector2(0,-Screen.height);
        gameObject.LeanMoveLocalY(0, 0.5f).setEase(LeanTweenType.easeInOutExpo);

    }
    public void OnDisabledWithDown() 
    {
        gameObject.LeanMoveLocalY(-Screen.height, 0.5f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(closeGameObject);

    }

    public void OnEnabledWithUpForHeader()
    {
        
        
        LeanTween.scale(gameObject.GetComponent<RectTransform>(), new Vector3(1, 1, 1), 0.6f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(openGameObject);

    }

}
