using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BMSObjectObserver : MonoBehaviour
{
    [SerializeField]
    protected TimelineSubject timelineSubject;

    protected virtual void Start()
    {
        timelineSubject.SubscribeTimeLine(OnTimeElapsed);
    }

    protected abstract void OnTimeElapsed(double elapsedTime);
}
