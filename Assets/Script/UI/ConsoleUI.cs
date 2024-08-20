using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using TMPro;

public class ConsoleUI : MonoBehaviour
{
    public GameObject logPrefab;  // Log mesajları için prefab
    public Transform logContainer; // ScrollView içindeki Content alanı
    public float logDuration = 5f; // Log mesajlarının ömrü


    private Dictionary<GameObject, float> logs = new Dictionary<GameObject, float>();
    private Dictionary<string, int> messageCount = new Dictionary<string, int>();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;

    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {



        // Zamanı dolmuş log mesajlarını sil
        List<GameObject> logsToRemove = new List<GameObject>();
        foreach (var log in logs)
        {

            if (Time.time > log.Value)
            {
                logsToRemove.Add(log.Key);
            }
        }

        foreach (var log in logsToRemove)
        {
            Destroy(log);
            logs.Remove(log);
        }

    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logMessage = FormatLogMessage(logString, stackTrace, type);

        if (messageCount.ContainsKey(logMessage))
        {
            messageCount[logMessage]++;
            UpdateLastLogMessage(logMessage);
        }
        else if(!messageCount.ContainsKey(logMessage))
        {
            messageCount[logMessage] = 1;
            CreateLogEntry(logMessage);
        }
    }

    string FormatLogMessage(string logString, string stackTrace, LogType type)
    {
        string logMessage = "";

        switch (type)
        {
            case LogType.Error:
                logMessage += "<color=red>[ERROR]</color> " + logString;
                break;
            case LogType.Assert:
                logMessage += "<color=orange>[ASSERT]</color> " + logString;
                break;
            case LogType.Warning:
                logMessage += "<color=yellow>[WARNING]</color> " + logString;
                break;
            case LogType.Log:
                logMessage += "<color=white>[LOG]</color> " + logString;
                break;
            case LogType.Exception:
                logMessage += "<color=red>[EXCEPTION]</color> " + logString + "\n" + stackTrace;
                break;
        }

        return logMessage;
    }
    void UpdateLastLogMessage(string logMessage)
    {
        foreach (Transform child in logContainer)
        {
            TextMeshProUGUI logText = child.GetComponent<TextMeshProUGUI>();
            if (logText.text.Contains(logMessage))
            {
                logText.text = logMessage + " (Tekrarlanan: " + messageCount[logMessage] + ")";
                break;
            }
        }
    }

    void CreateLogEntry(string logMessage)
    {
        GameObject newLog = Instantiate(logPrefab, logContainer);
        TextMeshProUGUI logText = newLog.GetComponent<TextMeshProUGUI>();
        logText.text = logMessage;
        logs.Add(newLog, Time.time + logDuration);
    }
}