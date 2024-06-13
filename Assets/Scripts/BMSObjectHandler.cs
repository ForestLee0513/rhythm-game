using System.Collections.Generic;
using BMS;
using UnityEngine;

public abstract class BMSObjectHandler : MonoBehaviour
{
    protected Queue<T>[] InitializeQueue<T>(List<T> targetObject) where T : BMSObject
    {
        Queue<T>[] bmsObjectQueue = new Queue<T>[targetObject.Count];
        // bgm 오브젝트 초기화
        foreach (var bmsObjectItem in targetObject)
        {
            if (bmsObjectQueue[bmsObjectItem.Bar] == null)
            {
                bmsObjectQueue[bmsObjectItem.Bar] = new Queue<T>();
            }

            bmsObjectQueue[bmsObjectItem.Bar].Enqueue(bmsObjectItem);
        }

        return bmsObjectQueue;
    }

    abstract protected void Start();

    abstract protected void Update();
}
