using BMS;
using UnityEngine;


public class BPMHandler : BMSObjectHandlerMultiThread
{
    private int currentBPMIndex = 0;
    private double currentBpm = 0;

    private double adjustedTiming;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnTimeElapsed(double elapsedMilliseconds)
    {
        if (currentBPMIndex < InGameManager.Instance.patternData.bpmList.Count)
        {
            var currentBPMEvent = InGameManager.Instance.patternData.bpmList[currentBPMIndex];
            var currentTiming = currentBPMEvent.Timing;

            var totalStopTime = InGameManager.Instance.patternData.CalculateStopTiming(currentBPMEvent, InGameManager.Instance.selectedTrack.stopTable);
            var adjustedTiming = currentTiming + totalStopTime;

            if (elapsedMilliseconds >= adjustedTiming)
            {
                currentBPMIndex++;
                InGameUIManager.Instance.UpdateBPMText(currentBPMEvent.Bpm);
            }
        }
    }
}
