using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float timer = 0;

    [SerializeField] private UITimer _uiTimer;
    private bool isRunning = false;
    
    private void Update()
    {
        if (!isRunning) 
            return;

        timer += Time.deltaTime;
        _uiTimer.UpdateTimerUI(timer);
    }

    //for pause state
    public void StopTimer()
    {
        isRunning = false;
    }
    
    //for resume state
    public void ResumeTimer()
    {
        timer = 0;
        isRunning = true;
    }

    // run when player finished the race
    public void StopTimerOnFinish()
    {
        StopTimer();
        GameSession.CurrentPlayerTime = timer;
    }
    
}
