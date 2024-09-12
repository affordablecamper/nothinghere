using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text timeText;
    private float startTime;
    private bool isRunning;

    private void Start()
    {
        startTime = Time.time;
        isRunning = true;
    }

    private void Update()
    {
        if (isRunning)
        {
            float elapsedTime = Time.time - startTime;
            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);
            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
            //timeText.text = timeString;
        }
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
        startTime = Time.time;
    }

    public float GetElapsedMinutes()
    {
        return (Time.time - startTime) / 60f;
    }

    public void ResetTimer()
    {
        startTime = Time.time;
    }
}