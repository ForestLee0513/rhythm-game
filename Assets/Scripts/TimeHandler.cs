using System;
using System.Diagnostics;
using System.Threading;

public class TimeSystem
{
    private Thread timeThread;
    protected Stopwatch stopwatch;
    private bool isRunning;
    
    public TimeSystem()
    {
        stopwatch = new Stopwatch();
        isRunning = false;
    }

    public void Start()
    {
        if (isRunning)
        {
            return;
        }

        isRunning = true;
        stopwatch.Start();
        timeThread = new Thread(TimeLoop);
        timeThread.Start();
    }

    public void Destroy()
    {
        if (!isRunning)
        {
            return;
        }

        isRunning = false;
        stopwatch.Stop();
        timeThread.Abort();
    }

    // 만들어 두긴했는데 지금 당장은 쓰지 않을거같음.. 그리고 테스트 해봐야함.
    // public void Reset()
    // {
    //     if (!isRunning)
    //     {
    //         return;
    //     }

    //     isRunning = false;
    //     stopwatch.Stop();
    //     timeThread.Join();
    //     timeThread.Start();
    // }

    private void TimeLoop()
    {
        while (isRunning)
        {
            OnTimeElapsed();
        }
    }

    protected virtual void OnTimeElapsed()
    {
    }
}
