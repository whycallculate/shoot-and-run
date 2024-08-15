using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;

public class Web : MonoBehaviour
{

    public string feedbackText;

    IEnumerator RegisterUser(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);


        using UnityWebRequest www = UnityWebRequest.Post("http://localhost/UnityBackendTutorial/Register.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }
    public IEnumerator Login(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/UnityBackendTutorial/Login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                feedbackText = "Connection error: " + www.error;
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);

                if (response.status == "success")
                {
                    feedbackText = "Login successful!";
                    ConnectToPhoton(response.user.username);
                    
                }
                else
                {
                    feedbackText = "Login failed: " + response.message;
                }
            }
        }
    }
    private void ConnectToPhoton(string username)
    {

        
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LoadLevel(1);
        PhotonNetwork.NickName = username;
    }

}
[System.Serializable]
public class LoginResponse
{
    public string message;
    public string status;
    public Data user;
}

