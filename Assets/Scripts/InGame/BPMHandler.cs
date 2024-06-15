using System.Collections.Generic;
using UnityEngine;

public class BPMHandler : BMSObjectHandler
{
    protected override void Start()
    {
        base.Start();
        bpmObjectQueue = InitializeQueue(InGameManager.Instance.patternData.bpmList);
    }

    Queue<BMS.BPM> bpmObjectQueue;

    protected override void Update()
    {
        base.Update();
        // if (bpmObjectQueue.Length > 0 && bpmObjectQueue[Metronome.Instance.BarCount] != null && bpmObjectQueue[Metronome.Instance.BarCount].Count > 0)
        // {
        //     if (Metronome.Instance.CurrentTime >= bpmObjectQueue[Metronome.Instance.BarCount].Peek().Timing)
        //     {
        //         Metronome.Instance.SetBpm(bpmObjectQueue[Metronome.Instance.BarCount].Dequeue().Bpm);
        //     }
        // }
    }
}
