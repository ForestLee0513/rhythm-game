using System.Collections.Generic;
using System.IO;
using BMS;
using UnityEngine;

public class BGAHandler : BMSObjectHandlerMultiThread
{
    private int currentBGAIndex = 0;

    protected override void OnTimeElapsed(double elapsedMilliseconds)
    {
        if (currentBGAIndex < InGameManager.Instance.patternData.bgaSequenceFrameList.Count && InGameManager.Instance.patternData.bgaSequenceFrameList[currentBGAIndex].Timing <= elapsedMilliseconds / 1000)
        {
            BGASequence.BGAFlagState flag = InGameManager.Instance.patternData.bgaSequenceFrameList[currentBGAIndex].Flag;
            int bgaKey = InGameManager.Instance.patternData.bgaSequenceFrameList[currentBGAIndex].BgaSequenceFrame;

            InGameUIManager.Instance.UpdateBGA(bgaKey, flag);
            currentBGAIndex++;
        }
    }
}
