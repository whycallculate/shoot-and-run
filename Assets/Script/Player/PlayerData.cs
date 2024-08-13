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
    public string PlayerDataToString()
    {
        
        string returnString = JsonUtility.ToJson(pmData);
        Debug.Log(returnString);
        return returnString;
        
    }
}

public class Data
{
    public Color BodyColor;
    public int Body;

}
