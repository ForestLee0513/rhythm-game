using BMS;
using UnityEngine;

public class BGMHandler : BMSObjectHandlerMultiThread 
{
    private int currentBGMIndex = 0;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnTimeElapsed(double elapsedMilliseconds)
    {
        FMODUnity.RuntimeManager.CoreSystem.update();
        if (currentBGMIndex < InGameManager.Instance.patternData.bgmKeySoundChannel.Count && InGameManager.Instance.patternData.bgmKeySoundChannel[currentBGMIndex].Timing <= elapsedMilliseconds / 1000)
        {
            InGameSoundManager.Instance.PlaySound(InGameManager.Instance.patternData.bgmKeySoundChannel[currentBGMIndex].KeySound);
            currentBGMIndex++;
        }

        foreach (Line line in InGameManager.Instance.patternData.lines)
        {
            if (line.NoteList.Count > 0)
            {
                if (line.NoteList[0].Timing <= elapsedMilliseconds / 1000)
                {
                    InGameSoundManager.Instance.PlaySound(line.NoteList[0].KeySound);
                    line.NoteList.RemoveAt(0);
                }
            }
        }
    }
}
