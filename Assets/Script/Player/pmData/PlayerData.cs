using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;

public class PlayerData : MonoBehaviour
{
    #region singliton
    private static PlayerData instance;
    public static PlayerData Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<PlayerData>();
            }
            return instance;
        }
    }
    #endregion
    public LoginResponse playerData;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void UpdateCharacterAppearance(int costumeIndex)
    {
        StartCoroutine(UpdateCharacterAppearanceCoroutine(playerData.user.id, costumeIndex));
        playerData.user.costume_index = costumeIndex;
    }

    private IEnumerator UpdateCharacterAppearanceCoroutine(int userId, int costumeIndex)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("costumeIndex", costumeIndex);

        using (UnityWebRequest www = UnityWebRequest.Post("https://whycallculate.online/game/UpdateCostume.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

}
[System.Serializable]
public class Data
{

    public int id;
    public string username;
    public string photon_userid;
    public int costume_index;
    public int character_level;



}
