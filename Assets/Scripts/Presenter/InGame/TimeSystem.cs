using System;
using System.Diagnostics;
using System.Threading;

public class TimeSystem
{
    private Thread timeThread;
    protected Stopwatch stopwatch;
    private bool isRunning;

    public event Action<double> OnTimeElapsed;

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
        timeThread = new Thread(TimeLoop);
        timeThread.Start();
        stopwatch.Start();
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
        OnTimeElapsed = null;
    }

    private void TimeLoop()
    {
        while (isRunning)
        {
            OnTimeElapsed?.Invoke(stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
