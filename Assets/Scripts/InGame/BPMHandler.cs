using System.Collections.Generic;
using UnityEngine;

public class BPMHandler : BMSObjectHandlerMultiThread
{
    private int currentBPMIndex = 0;
    private double currentBpm = 0;

    protected override void Start()
    {
        base.Start();
        currentBpm = InGameManager.Instance.patternData.bpmList[currentBPMIndex].Bpm;
    }

    protected override void OnTimeElapsed(double elapsedMilliseconds)
    {
        if (currentBPMIndex < InGameManager.Instance.patternData.bpmList.Count && InGameManager.Instance.patternData.bpmList[currentBPMIndex].Timing <= elapsedMilliseconds / 1000)
        {
            currentBpm = InGameManager.Instance.patternData.bpmList[currentBPMIndex].Bpm;
            Debug.Log($"현재 bpm: {currentBpm}");
            currentBPMIndex++;
        }
    }
}
