using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using BMS;

public class InGameManager : MonoBehaviour
{
    public Pattern patternData = null;
    private static InGameManager instance;
    public static InGameManager Instance { get { return instance; } }

    // 노래와 관련된 정보
    private double bpm = 0.0f;
    // 메트로놈 틱 흐른 횟수
    private int bar = 0;
    // 다음 틱
    private double nextTick = 0;
    private double tickInterval = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // 개발 환경인 경우에는 로컬에 있는 임의의 파일로 지정함.
        #if UNITY_EDITOR
        BMSHeaderParser parsedHeaderData = new BMSHeaderParser(Path.Combine(Application.dataPath, "bmsFiles/Merry Christmas Mr. (by sasakure.UK)/Merry Christmas Mr.Lawrence.bms"));
        patternData = new BMSMainDataParser(parsedHeaderData.TrackInfo).Pattern;
        InGameSoundManager.Instance.LoadSounds(parsedHeaderData.TrackInfo);
        #elif UNITY_STANDALONE
        patternData = new BMSMainDataParser(GameManager.Instance.selectedTrack).Pattern;
        currentBPM = GameManager.Instance.selectedTrack.TrackInfo.bpm;
        InGameSoundManager.Instance.LoadSounds(GameManager.instance.selectedTrack);
        #endif
    }
}
