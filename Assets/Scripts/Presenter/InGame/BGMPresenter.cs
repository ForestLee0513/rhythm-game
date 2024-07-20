public class BGMPresenter : BMSObjectObserver 
{
    private int currentBGMIndex = 0;

    protected override void OnTimeElapsed(double elapsedTime)
    {
        FMODUnity.RuntimeManager.CoreSystem.update();
        if (currentBGMIndex < InGameManager.Instance.patternData.bgmKeySoundChannel.Count && InGameManager.Instance.patternData.bgmKeySoundChannel[currentBGMIndex].Timing <= elapsedTime)
        {
            InGameSoundManager.Instance.PlaySound(InGameManager.Instance.patternData.bgmKeySoundChannel[currentBGMIndex].KeySound);
            currentBGMIndex++;
        }

        for (int i = 0; i < InGameManager.Instance.patternData.lines.Length; ++i)
        {
            if (InGameManager.Instance.patternData.lines[i].NoteList.Count > 0 && InGameManager.Instance.patternData.lines[i].NoteList[0].Timing <= elapsedTime)
            {
                InGameSoundManager.Instance.PlaySound(InGameManager.Instance.patternData.lines[i].NoteList[0].KeySound);
                //Debug.Log($"{i}: {InGameManager.Instance.patternData.lines[i].NoteList[0].Flag} {InGameManager.Instance.patternData.lines[i].NoteList[0].KeySound}");
                InGameManager.Instance.patternData.lines[i].NoteList.RemoveAt(0);
            }
        }
    }
}
