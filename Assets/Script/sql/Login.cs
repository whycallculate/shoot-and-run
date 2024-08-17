using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{

    private void Start()
    {
        headerObject.SetActive(true);
        headerObject.GetComponent<UIAnim>().OnEnabledWithUpForHeader();
        
    }
    [Header("Header")]
    [SerializeField] private GameObject headerObject;


    [Header("LoginStuff")]
    [SerializeField] private GameObject loginBox;
    [SerializeField] private TMP_InputField lgUserNameInput;
    [SerializeField] private TMP_InputField lgPasswordInput;

    [Header("RegisterStuff")]
    [SerializeField] private GameObject registerBox;
    [SerializeField] private TMP_InputField regUserNameInput;
    [SerializeField] private TMP_InputField regPasswordInput;

    public void LoginButtonMethod()
    {


        StartCoroutine(MainWeb.Instance.web.Login(lgUserNameInput.text, lgPasswordInput.text));
        StartCoroutine(MainWeb.Instance.web.HeartbeatCoroutine(lgUserNameInput.text));
    }
    public void RegisterButtonMethod()
    {
        StartCoroutine(MainWeb.Instance.web.RegisterUser(regUserNameInput.text, regPasswordInput.text));
    }
    public void OpenRegisterBox()
    {
        if(registerBox.activeSelf == false)
        {
            registerBox.GetComponent<UIAnim>().OnEnabledWithUp();
            loginBox.GetComponent<UIAnim>().OnDisabledWithDown();
        }
        
    }
    public void OpenLoginBox()
    {
        if (loginBox.activeSelf == false)
        {
            
            registerBox.GetComponent<UIAnim>().OnDisabledWithDown();
            loginBox.GetComponent<UIAnim>().OnEnabledWithUp();
        }
    }
}
