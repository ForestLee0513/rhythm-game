using BMS;
using UnityEngine;

public class BGAPresenter : BMSObjectObserver
{
    private int currentBGAIndex = 0;
    private int currentLayerBGAIndex = 0;
    private int currentVideoBGAIndex = 0;

    protected override void OnTimeElapsed(double elapsedTime)
    {
        if (InGameUIManager.Instance == null)
        {
            return;
        }

        if (currentVideoBGAIndex < InGameManager.Instance.patternData.videoBGAList.Count && InGameManager.Instance.patternData.videoBGAList[currentVideoBGAIndex].Timing <= elapsedTime)
        {
            int bgaKey = InGameManager.Instance.patternData.videoBGAList[currentBGAIndex].BgaSequenceFrame;

            InGameUIManager.Instance.UpdateVideoBGA(bgaKey);
            currentVideoBGAIndex++;
        }

        // Base BGA
        if (currentBGAIndex < InGameManager.Instance.patternData.bgaSequenceFrameList.Count && InGameManager.Instance.patternData.bgaSequenceFrameList[currentBGAIndex].Timing <= elapsedTime)
        {
            BGASequence.BGAFlagState flag = InGameManager.Instance.patternData.bgaSequenceFrameList[currentBGAIndex].Flag;
            int bgaKey = InGameManager.Instance.patternData.bgaSequenceFrameList[currentBGAIndex].BgaSequenceFrame;

            InGameUIManager.Instance.UpdateBaseBGA(bgaKey, flag);
            currentBGAIndex++;
        }

        // Layer BGA
        if (currentLayerBGAIndex < InGameManager.Instance.patternData.layerBGASequenceFrameList.Count && InGameManager.Instance.patternData.layerBGASequenceFrameList[currentLayerBGAIndex].Timing <= elapsedTime)
        {
            BGASequence.BGAFlagState flag = InGameManager.Instance.patternData.layerBGASequenceFrameList[currentLayerBGAIndex].Flag;
            int bgaKey = InGameManager.Instance.patternData.layerBGASequenceFrameList[currentLayerBGAIndex].BgaSequenceFrame;

            InGameUIManager.Instance.UpdateLayerBGA(bgaKey, flag);
            currentLayerBGAIndex++;
        }
    }
}
