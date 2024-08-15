using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [Header("LoginStuff")]
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TMP_InputField passwordInput;
    

    public void LoginButtonMethod()
    {
        StartCoroutine(MainWeb.Instance.web.Login(userNameInput.text, passwordInput.text));
    }
}
