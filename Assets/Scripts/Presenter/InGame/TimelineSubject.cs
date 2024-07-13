using System;
using UnityEngine;

public class TimelineSubject : MonoBehaviour
{
    TimeSystem timeSystem;

    private void Awake()
    {
        if (timeSystem == null)
        {
            timeSystem = new();
        }
    }

    public void StartTime()
    {
        timeSystem.Start();
    }

    public void SubscribeTimeLine(Action<double> action)
    {
        timeSystem.OnTimeElapsed += action;
    }

    public void UnSubscribeTimeLine(Action<double> action)
    {
        timeSystem.OnTimeElapsed -= action;
    }

    private void OnDestroy()
    {
        timeSystem.Destroy();
    }
}
