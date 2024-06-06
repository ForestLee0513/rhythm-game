using System.Collections.Generic;
using BMS;
using UnityEngine;

public class BMSObjectHandler : MonoBehaviour
{
    // bms 오브젝트의 마디를 index로 관리
    Queue<BMS.Note>[] bgmQueue;

    // 초기화
    void Start()
    {
        bgmQueue = new Queue<BMS.Note>[InGameManager.Instance.patternData.bgmKeySoundChannel.Count];
        // bgm 오브젝트 초기화
        foreach (var bgmSoundChannel in InGameManager.Instance.patternData.bgmKeySoundChannel)
        {
            if (bgmQueue[bgmSoundChannel.Bar] == null)
            {
                bgmQueue[bgmSoundChannel.Bar] = new Queue<BMS.Note>();
            }

            bgmQueue[bgmSoundChannel.Bar].Enqueue(bgmSoundChannel);
        }

        // Debug.Log(Metronome.Instance);
    }

    void Update()
    {
        // if (bgmQueue[Metronome.Instance.BarCount] != null)
        // {
        //     Debug.Log(Metronome.Instance.BeatInterval);
        // }
    }
}
