using UnityEngine;
using System.Collections.Generic;
using System;

public class Logger : MonoBehaviour
{
    [SerializeField] private int fontSize = 24;
    private GUIStyle GUIStyle = new GUIStyle();

    private void Awake()
    {
        GUIStyle.fontSize = fontSize;
    }

    static Queue<string> queue = new Queue<string>(6);

	void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(25, 25, Screen.width, 140));
		foreach (string s in queue) {
			GUILayout.Label(s, GUIStyle);
		}
		GUILayout.EndArea();
	}

	void HandleLog(string message, string stackTrace, LogType type)
	{
        if (type == LogType.Log)
            queue.Enqueue($"<color=\"white\">{DateTime.Now.ToString("HH:mm:ss")} {message}</color>");
        else if (type == LogType.Warning)
            queue.Enqueue($"<color=\"yellow\">{DateTime.Now.ToString("HH:mm:ss")} {message}</color>");
        else if (type == LogType.Error)
            queue.Enqueue($"<color=\"red\">{DateTime.Now.ToString("HH:mm:ss")} {message}</color>");

        if (queue.Count > 10)
        {
            queue.Dequeue();
        }

    }
}