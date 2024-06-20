using System.Collections.Generic;
using System.IO;
using BMS;
using UnityEngine;

public class BGAHandler : BMSObjectHandlerMultiThread
{
    // Dict로 저장 후 시간에 따라 BGA 변경
    // Dict의 키를 기반으로 Index 생성 후 Index의 값으로 Dict의 값 참조
    // Start에서 런타임에서 텍스쳐 로드처리
    // mp4와 같은 영상파일 / 이미지 파일에 대한 예외처리 필수

    private int currentBGAIndex = 0;
    // private Dictionary<double, >

    private string[] imageExtensions = { ".png", ".jpg", ".bmp" };

    protected override void Start()
    {
        base.Start();

        foreach (BGASequence bgaSequence in InGameManager.Instance.patternData.bgaSequenceFrameList)
        {
            foreach (string extension in imageExtensions)
            {
                // if (File.Exists($"{trackInfo.audioFileNames[trackSoundKey]}{extension}"))
                // {
                //     selectedExtension = extension;
                //     break;
                // }
            }
        }
        
    }

    protected override void OnTimeElapsed(double elapsedMilliseconds)
    {
        if (currentBGAIndex < InGameManager.Instance.patternData.bpmList.Count && InGameManager.Instance.patternData.bpmList[currentBGAIndex].Timing <= elapsedMilliseconds / 1000)
        {
            // currentBpm = InGameManager.Instance.patternData.bpmList[currentBGAIndex].Bpm;
            // Debug.Log($"현재 bpm: {currentBpm}");
            // currentBGAIndex++;
        }
    }
}
