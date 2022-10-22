using UnityEngine;

public class TimerSystem 
{
    private float _timer;
    public float Timer { get { return _timer; } }
    
    public delegate void TimerEnd();
    public event TimerEnd OnTimerEnd;
    
    public TimerSystem(float startingTimer)
    {
        _timer = startingTimer;
    }
    
    /// <summary>
    /// Adds time to the timer by the given amount.
    /// </summary>
    /// <param name="amount"></param>
    public void AddTime(float amount)
    {
        if(amount < 0)
        {
            Debug.LogError("Cannot add negative time.");
            return;
        }
        _timer += amount;
    }
    
    /// <summary>
    /// Removes time from the timer by the given amount.
    /// Incoming amount must be positive.
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveTime(float amount)
    {
        if(amount < 0)
        {
            Debug.LogError("Cannot remove negative time.");
            return;
        }
        _timer -= amount;
        if(_timer <= 0)
        {
            _timer = 0;
            OnTimerEnd?.Invoke();
        }
    }
    
    /// <summary>
    /// Call in update to count down the timer.
    /// </summary>
    public void Update()
    {
        if(_timer > 0)
        {
            RemoveTime(Time.deltaTime);
        }
    }

}