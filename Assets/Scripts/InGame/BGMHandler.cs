using System.Collections.Generic;

public class BGMHandler : BMSObjectHandler
{
    protected override void Start()
    {
        bgmObjectQueue = InitializeQueue(InGameManager.Instance.patternData.bgmKeySoundChannel);
    }

    Queue<BMS.Note>[] bgmObjectQueue;

    protected override void Update()
    {
        if (bgmObjectQueue[Metronome.Instance.BarCount] != null && bgmObjectQueue[Metronome.Instance.BarCount].Count > 0)
        {
            if (Metronome.Instance.CurrentTime >= bgmObjectQueue[Metronome.Instance.BarCount].Peek().Timing)
            {
                InGameSoundManager.Instance.PlaySound(bgmObjectQueue[Metronome.Instance.BarCount].Dequeue().KeySound);
            }
        }
    }

}
