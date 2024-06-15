using System.Collections.Generic;
using BMS;
using UnityEngine;

public abstract class BMSObjectHandler : MonoBehaviour
{
    protected int bar = 0;
    protected double startedTime;
    protected double currentTime;

    protected Queue<T> InitializeQueue<T>(List<T> targetObject) where T : BMSObject
    {
        Queue<T> bmsObjectQueue = new();

        // bgm 오브젝트 초기화
        foreach (var bmsObjectItem in targetObject)
        {
            bmsObjectQueue.Enqueue(bmsObjectItem);
        }

        return bmsObjectQueue;
    }

    protected virtual void Start()
    {
        startedTime = Time.time;
    }

    protected virtual void Update()
    {
        currentTime = Time.time - startedTime;
    }
}
