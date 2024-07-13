using System.Collections.Generic;
using System.Linq;
using BMS;
using JetBrains.Annotations;
using UnityEngine;

public abstract class BMSObjectHandlerMultiThread : MonoBehaviour
{
    private CustomTimeHandler timeHandler;

    protected virtual void Start()
    {
        timeHandler = new CustomTimeHandler(this);
        timeHandler.Start();
    }

    protected virtual void OnDestroy()
    {
        timeHandler.Destroy();
    }

    protected abstract void OnTimeElapsed(double elapsedMilliseconds);

    private class CustomTimeHandler : TimeSystem
    {
        private readonly BMSObjectHandlerMultiThread handler;

        public CustomTimeHandler(BMSObjectHandlerMultiThread handler) : base()
        {
            this.handler = handler;
        }

        //protected override void OnTimeElapsed()
        //{
        //    handler.OnTimeElapsed(stopwatch.Elapsed.TotalMilliseconds);
        //}
    }
}
