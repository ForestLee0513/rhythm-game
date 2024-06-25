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
        double elapsedSeconds = elapsedMilliseconds / 1000;
        if (currentBPMIndex < InGameManager.Instance.patternData.bpmList.Count)
        {
            var currentBPMEvent = InGameManager.Instance.patternData.bpmList[currentBPMIndex];
            var currentTiming = currentBPMEvent.Timing;

            var totalStopTime = InGameManager.Instance.patternData.CalculateStopTiming(currentBPMEvent, InGameManager.Instance.selectedTrack.stopTable);
            var adjustedTiming = currentTiming + totalStopTime;

            if (elapsedSeconds >= adjustedTiming)
            {
                currentBPMIndex++;
                Debug.Log($"Incremented currentBPM to {currentBPMEvent.Bpm} due to adjusted timing.");
            }
        }
    }
}
