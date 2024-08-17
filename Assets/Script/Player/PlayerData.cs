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
        pmData = new Data();
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        
    }
    public string PlayerDataToString()
    {
        //pmData.username = PlayerPrefs.GetString("nickName");
        pmData.Body = PlayerPrefs.GetInt("Body");
        string returnString = JsonUtility.ToJson(pmData);
        
        return returnString;
    }
}
[System.Serializable]
public class Data
{

    public int id;
    public string username;
    public string photon_userid;



    public Color BodyColor;
    public int Body;

}
