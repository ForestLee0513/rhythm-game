using System.Collections.Generic;
using BMS;
using UnityEngine;

public abstract class BMSObjectHandler : MonoBehaviour
{
    // bms 오브젝트의 마디를 index로 관리
    protected Queue<BMS.Note>[] bmsObjectQueue;

    // 초기화
    void Start()
    {
        bmsObjectQueue = new Queue<BMS.Note>[InGameManager.Instance.patternData.bgmKeySoundChannel.Count];
        // bgm 오브젝트 초기화
        foreach (var bgmSoundChannel in InGameManager.Instance.patternData.bgmKeySoundChannel)
        {
            if (bmsObjectQueue[bgmSoundChannel.Bar] == null)
            {
                bmsObjectQueue[bgmSoundChannel.Bar] = new Queue<BMS.Note>();
            }

            bmsObjectQueue[bgmSoundChannel.Bar].Enqueue(bgmSoundChannel);
        }
    }

    abstract public void Update();
}
