using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Data pmData;

    private void OnEnable()
    {
        Debug.Log("Burasi Calisti");
        pmData = new Data();
        DontDestroyOnLoad(gameObject);
        

    }
    private void Update()
    {
        Debug.Log(PlayerDataToString());
        PlayerPrefs.Save();
    }
    public string PlayerDataToString()
    {
        pmData.nickName = PlayerPrefs.GetString("nickName");
        pmData.Body = PlayerPrefs.GetInt("Body");
        string returnString = JsonUtility.ToJson(pmData);
        Debug.Log(returnString);
        return returnString;
        
    }

}

public class Data
{
    [Header("PlayerData")]
    public string nickName;
    int playerLevel;
    Sprite avatar;

    [Header("PlayerCustomizeData")]
    public Color BodyColor;
    public int Body;

}
