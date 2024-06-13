public class BGMHandler : BMSObjectHandler
{
    public override void Update()
    {
        if (bmsObjectQueue[Metronome.Instance.BarCount] != null && bmsObjectQueue[Metronome.Instance.BarCount].Count > 0)
        {
            if (Metronome.Instance.CurrentTime >= bmsObjectQueue[Metronome.Instance.BarCount].Peek().Timing)
            {
                InGameSoundManager.Instance.PlaySound(bmsObjectQueue[Metronome.Instance.BarCount].Dequeue().KeySound);
            }
        }
    }
}
