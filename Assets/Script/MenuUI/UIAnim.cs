using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnim : MonoBehaviour
{

    public void OnEnabled()
    {
        gameObject.SetActive(true);
        LeanTween.scale(gameObject.GetComponent<RectTransform>(), new Vector3(1, 1, 1), 0.3f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(openGameObject);
    }
    void openGameObject()
    {
        gameObject.SetActive(true);
    }
    public void OnDisabled()
    {

        LeanTween.scale(gameObject.GetComponent<RectTransform>(), new Vector3(0, 0, 0), 0.3f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(closeGameObject);
    }
    void closeGameObject()
    {
        gameObject.SetActive(false);
    }
}
