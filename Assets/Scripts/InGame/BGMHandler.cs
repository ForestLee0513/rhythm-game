using System.Collections.Generic;
using System.Linq;
using BMS;
using UnityEngine;

public class BGMHandler : BMSObjectHandler
{
    protected override void Start()
    {
        base.Start();
        bgmObjectQueue = InitializeQueue(InGameManager.Instance.patternData.bgmKeySoundChannel);

        note1 = InitializeQueue(InGameManager.Instance.patternData.lines[0].NoteList);
        note2 = InitializeQueue(InGameManager.Instance.patternData.lines[1].NoteList);
        note3 = InitializeQueue(InGameManager.Instance.patternData.lines[2].NoteList);
        note4 = InitializeQueue(InGameManager.Instance.patternData.lines[3].NoteList);
        note5 = InitializeQueue(InGameManager.Instance.patternData.lines[4].NoteList);
        note6 = InitializeQueue(InGameManager.Instance.patternData.lines[5].NoteList);
        note7 = InitializeQueue(InGameManager.Instance.patternData.lines[6].NoteList);
        note8 = InitializeQueue(InGameManager.Instance.patternData.lines[7].NoteList);
    }

    Queue<BMS.Note> bgmObjectQueue;
    Queue<BMS.Note> note1;
    Queue<BMS.Note> note2;
    Queue<BMS.Note> note3;
    Queue<BMS.Note> note4;
    Queue<BMS.Note> note5;
    Queue<BMS.Note> note6;
    Queue<BMS.Note> note7;
    Queue<BMS.Note> note8;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (bgmObjectQueue.Count > 0 && currentTime >= bgmObjectQueue.Peek().Timing)
        {
            InGameSoundManager.Instance.PlaySound(bgmObjectQueue.Dequeue().KeySound);
        }

        if (note1.Count > 0 && currentTime >= note1.Peek().Timing)
        {
            InGameSoundManager.Instance.PlaySound(note1.Dequeue().KeySound);
        }

        if (note2.Count > 0 && currentTime >= note2.Peek().Timing)
        {
            InGameSoundManager.Instance.PlaySound(note2.Dequeue().KeySound);
        }

        if (note3.Count > 0 && currentTime >= note3.Peek().Timing)
        {
            InGameSoundManager.Instance.PlaySound(note3.Dequeue().KeySound);
        }

        if (note4.Count > 0 && currentTime >= note4.Peek().Timing)
        {
            InGameSoundManager.Instance.PlaySound(note4.Dequeue().KeySound);
        }

        if (note5.Count > 0 && currentTime >= note5.Peek().Timing)
        {
            InGameSoundManager.Instance.PlaySound(note5.Dequeue().KeySound);
        }

        if (note6.Count > 0 && currentTime >= note6.Peek().Timing)
        {
            InGameSoundManager.Instance.PlaySound(note6.Dequeue().KeySound);
        }

        if (note7.Count > 0 && currentTime >= note7.Peek().Timing)
        {
            InGameSoundManager.Instance.PlaySound(note7.Dequeue().KeySound);
        }

        if (note8.Count > 0 && currentTime >= note8.Peek().Timing)
        {
            InGameSoundManager.Instance.PlaySound(note8.Dequeue().KeySound);
        }
    }
}
