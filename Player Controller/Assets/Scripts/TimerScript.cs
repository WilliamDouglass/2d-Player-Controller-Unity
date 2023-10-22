using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float timeElapsed;

    [SerializeField] private bool timerIsRunning;
    [SerializeField] private bool resetTimer;


    void Start()
    {
    }

    void Update()
    {
        if (resetTimer)
        {
            resetTimer = false;
            timerText.text = "0:00";
            timeElapsed = 0;
        }

        if (timerIsRunning)
        {
            timeElapsed += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeElapsed / 60);
            int seconds = Mathf.FloorToInt(timeElapsed % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    public void ResetTimer()
    {
        resetTimer = true;
    }
    public void StartTimer()
    {
        timerIsRunning = true;
    }
    public void StopTimer()
    {
        timerIsRunning = false;
    }
}
