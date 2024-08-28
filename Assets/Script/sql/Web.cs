using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class Web : MonoBehaviour
{
    public string username, userID;
    public string feedbackText;
    public GameObject sceneAnim;
    public GameObject alertPanel;
    int costumeIndex;
    int characterLevel;
    private void Start()
    {
        
    }
    public IEnumerator RegisterUser(string username, string password)
    {
        PhotonNetwork.AuthValues = new AuthenticationValues();
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        // PhotonUserId'yi buraya ekleyin
        PhotonNetwork.AuthValues.UserId = username;
        string photonUserId = PhotonNetwork.AuthValues.UserId;
        form.AddField("photonUserId", photonUserId);

        using (UnityWebRequest www = UnityWebRequest.Post("https://whycallculate.online/Register.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                feedbackText = "Connection error: " + www.error;
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Received JSON: " + jsonResponse);

                try
                {
                    RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(jsonResponse);
                    if (response.status == "success")
                    {
                        feedbackText = "Registration successful!";
                    }
                    else
                    {
                        feedbackText = "Registration failed: " + response.message;
                    }
                }
                catch (System.Exception ex)
                {
                    feedbackText = "JSON parse error: " + ex.Message;
                    Debug.LogError("JSON parse error: " + ex.Message);
                }
            }
        }
    }

    public IEnumerator Login(string username, string password)
    {
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = username;
        string photonUserId = PhotonNetwork.AuthValues.UserId;


        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("photonUserId", photonUserId);


        using (UnityWebRequest www = UnityWebRequest.Post("https://whycallculate.online/Login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Login failed: " + www.error);
                AlertPanel.Instance.ShowLog(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Login response: " + jsonResponse);

                LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);


                if (response.status == "success")
                {

                    PhotonNetwork.AuthValues.UserId = response.user.photon_userid;
                    PhotonNetwork.NickName = response.user.username;

                    ConnectToPhoton(response.user.username,response.user.photon_userid,response);

                }
                else if (response.status == "fail" && response.message == "User is already logged in")
                {
                    AlertPanel.Instance.ShowLog("User is already logged in.Please log out from other devices.");
                   
                }
                else
                {
                    AlertPanel.Instance.ShowLog(response.message);
                    Debug.LogWarning("Login failed: " + response.message);
                }
            }
        }
    }
    public IEnumerator UpdateOnlineStatus(bool isOnline)
    {
        WWWForm form = new WWWForm();
        form.AddField("photonUserId", PhotonNetwork.AuthValues.UserId);
        form.AddField("isOnline", isOnline ? 1 : 0);

        using (UnityWebRequest www = UnityWebRequest.Post("http://whycallculate.online/UpdateOnlineStatus.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("UpdateOnlineStatus failed: " + www.error);
            }
        }
    }
    public IEnumerator Logout(string username)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);

        using (UnityWebRequest www = UnityWebRequest.Post("https://whycallculate.online/Logout.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Logout failed: " + www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Logout response: " + jsonResponse);
                

                RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(jsonResponse);

                if (response.status == "success")
                {
                    Debug.Log("Logout successful!");
                }
                else
                {
                    Debug.LogWarning("Logout failed: " + response.message);
                }
            }
        }
    }

    public IEnumerator HeartbeatCoroutine(string name)
    {
        
        while (true)
        {
            WWWForm form = new WWWForm();
            form.AddField("photonUserId", name);
            
            using (UnityWebRequest www = UnityWebRequest.Post("https://whycallculate.online/Heartbeat.php", form))
            {
                yield return www.SendWebRequest();
                
                if (www.result != UnityWebRequest.Result.Success)
                {
                    yield return new WaitForSeconds(5f);
                    Debug.LogError("Heartbeat failed: " + www.error);
                    break;
                    
                }
                
            }

            // 5 saniyede bir kalp atışı gönder
            yield return new WaitForSeconds(9f);
        }
    }


    private void ConnectToPhoton(string name, string ID,LoginResponse userData)
    {
        username = name;
        this.userID = ID;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = name;
        sceneAnim.GetComponent<UIAnim>().ChangeScene(sceneAnim, 1);
        PlayerData.Instance.playerData = userData;
        Debug.Log(userData.user.character_level);
        Debug.Log(userData.user.costume_index);
    }

}
[System.Serializable]
public class LoginResponse
{
    public string message;
    public string status;
    public Data user;
}

[System.Serializable]
public class RegisterResponse
{
    public string status;
    public string message;
}

