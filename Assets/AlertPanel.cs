using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertPanel : MonoBehaviour
{
    [SerializeField] GameObject alertPanel;
    private static AlertPanel instance;
    public static AlertPanel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AlertPanel>();
            }
            return instance;
        }
    }
    public void ShowLog(string text)
    {
        alertPanel.SetActive(true);
        alertPanel.GetComponent<UIAnim>().OnEnabled();
        alertPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = text;

    }
    public void CloseLog()
    {
        alertPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = " ";
        alertPanel.GetComponent<UIAnim>().OnDisabled();
    }
}
